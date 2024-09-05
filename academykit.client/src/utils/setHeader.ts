export const setHeader = () => {
  const info =
    localStorage.getItem("app-info") &&
    JSON.parse(localStorage.getItem("app-info") ?? "Academy kit");
  if (info) {
    let link = document.querySelector("link[rel~='icon']") as HTMLLinkElement;
    document.title = info.name;
    if (!link) {
      link = document.createElement("link");
      link.rel = "icon";
      document.getElementsByTagName("head")[0].appendChild(link);
    }
    link.href = info.logo;
  }
};
