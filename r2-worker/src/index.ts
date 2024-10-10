/**
 * Welcome to Cloudflare Workers! This is your first worker.
 *
 * - Run `wrangler dev src/index.ts` in your terminal to start a development server
 * - Open a browser tab at http://localhost:8787/ to see your worker in action
 * - Run `wrangler publish src/index.ts --name my-worker` to publish your worker
 *
 * Learn more at https://developers.cloudflare.com/workers/
 */

// export default {
// 	async fetch(
// 		request: Request,
// 		env: Env,
// 		ctx: ExecutionContext
// 	): Promise<Response> {
// 		return new Response("Hello World!");
// 	},
// };
interface Env {
  BUCKET_VIDEOS_DEV: R2Bucket;
  BUCKET_VIDEOS: R2Bucket;
  BUCKET_PHOTOS_DEV: R2Bucket;
  BUCKET_PHOTOS: R2Bucket;

  BUCKET_PREFIX_NAME: string;
  DOMAIN: string;
  ALLOWED_DOMAINS: string[]; // Array of allowed domains

  [key: string]: R2Bucket | string | string[];
}

function parseRange(
  encoded: string | null
): undefined | { offset: number; length?: number } {
  if (encoded === null || !encoded.startsWith("bytes=")) {
    return;
  }

  const rangeParts = encoded.split("bytes=")[1].split("-");
  if (rangeParts.length !== 2) {
    throw new Error("Range header is not in the correct format");
  }
  const start = rangeParts[0];
  const end = rangeParts[1];

  return {
    offset: parseInt(start, 10),
    length: end ? parseInt(end, 10) - parseInt(start, 10) + 1 : undefined,
  };
}

function objectNotFound(objectName: string): Response {
  return new Response(
    `<html><body>R2 object "<b>${objectName}</b>" not found</body></html>`,
    {
      status: 404,
      headers: {
        "content-type": "text/html; charset=UTF-8",
      },
    }
  );
}

function isRefererValid(ref: string | null, allowedDomains: string[]): boolean {
  if (!ref || ref.trim() === "") return true; // No Referer means not allowed

  const referer = new URL(ref);
  return allowedDomains.some(
    (domain) =>
      referer.hostname === domain || referer.hostname.endsWith(`.${domain}`)
  );
}

function getBucket(bucketName: string, env: Env) {
  return env!["BUCKET_" + bucketName] as R2Bucket;
}

const handler: ExportedHandler<Env> = {
  async fetch(request: Request, env: Env): Promise<Response> {
    const url = new URL(request.url);
    const refererHeader = request.headers.get("Referer");

    // Validate the Referer against allowed domains
    const allowedDomains = env.ALLOWED_DOMAINS as string[];
    if (!isRefererValid(refererHeader, allowedDomains)) {
      return new Response("Forbidden", { status: 403 });
    }

    const bucketAndObject = url.pathname.split("/"); // e.g Array(3)["", "gv-videos-dev", "2ea9ce66-4731-4dd8-9d16-3ed166078d8c-720p.mp4"]
    if (bucketAndObject.length !== 3) return objectNotFound(url.pathname);

    const bucketName: string = bucketAndObject[1]
      .substring(env.BUCKET_PREFIX_NAME.length + 1) // We deduct appname because bucket variables are named generically without appnames.
      .toUpperCase()
      .replaceAll("-", "_");

    const fileName: string = bucketAndObject[2];

    if (!Object.keys(env).includes("BUCKET_" + bucketName) || fileName === "")
      return objectNotFound(`${bucketName}/${fileName}`);

    if (request.method === "GET") {
      const range = parseRange(request.headers.get("range"));
      const object = await getBucket(bucketName, env).get(fileName, {
        range,
        onlyIf: request.headers,
      });

      if (object === null) {
        return objectNotFound(`${bucketName}/${fileName}`);
      }

      const headers = new Headers();
      object.writeHttpMetadata(headers);
      // headers.set("etag", object.httpEtag);
      headers.set("accept-ranges", "bytes");

      if (range && object.size) {
        let endRange = range.length
          ? range.offset + range.length - 1
          : object.size - 1;
        headers.set(
          "Content-Range",
          `bytes ${range.offset}-${endRange}/${object.size}`
        );
      }

      headers.set("Cache-Control", "public, max-age=2592000, immutable");
      headers.set(
        "Content-Security-Policy",
        `default-src 'self' *.${env.DOMAIN} ${env.DOMAIN}`
      );

      const status = (object as R2ObjectBody).body ? (range ? 206 : 200) : 304;
      return new Response((object as R2ObjectBody).body, {
        headers,
        status,
      });
    }

    if (request.method === "HEAD") {
      const object = await getBucket(bucketName, env).head(fileName);

      if (object === null) {
        return objectNotFound(`${bucketName}/${fileName}`);
      }

      const headers = new Headers();
      object.writeHttpMetadata(headers);
      headers.set("accept-ranges", "bytes");
      headers.set(
        "Content-Security-Policy",
        `default-src 'self' *.${env.DOMAIN} ${env.DOMAIN}`
      );
      headers.set("Cache-Control", "public, max-age=2592000");

      return new Response(null, {
        headers,
      });
    }

    return new Response(`Unsupported method`, {
      status: 400,
    });
  },
};

export default handler;
