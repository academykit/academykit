import sanitizeHtml from "sanitize-html";

function removeTags(input: string) {
  return sanitizeHtml(input);
}

export default removeTags;
