export function checkValidUrl(url: string) {
  const pattern =
    /^(https?:\/\/)?([a-zA-Z0-9-.]+\.)+[a-zA-Z]{2,}(:\d+)?(\/[^\s]*)?$/;
  return pattern.test(url);
}
