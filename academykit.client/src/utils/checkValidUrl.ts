export function checkValidUrl(url: string) {
  const pattern =
    /^(https?:\/\/)?[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)+(:\d+)?(\/[\w\-./~%]*)?$/;
  return pattern.test(url);
}
