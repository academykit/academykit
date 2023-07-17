function stripHTML(html: string) {
  //   let doc = new DOMParser().parseFromString(html, "text/html");
  //   return doc.body.textContent || "";

  const a = document.createElement('div');
  a.innerHTML = html;
  return a.innerText ?? '';
}

export default stripHTML;
