import { checkValidUrl } from "./checkValidUrl";
import { APP_INFO_LOCAL_STORAGE_KEY } from "./constants";

export const setHeader = ({
  name,
  logoUrl,
}: {
  name: string;
  logoUrl: string;
}) => {
  const info =
    localStorage.getItem(APP_INFO_LOCAL_STORAGE_KEY) &&
    JSON.parse(
      localStorage.getItem(APP_INFO_LOCAL_STORAGE_KEY) ?? "Academy kit"
    );
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

  localStorage.setItem(
    APP_INFO_LOCAL_STORAGE_KEY,
    JSON.stringify({
      name: name ?? "AcademyKit",
      logo: checkValidUrl(logoUrl) ? logoUrl : "/favicon.png",
    })
  );
};
