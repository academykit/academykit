import { IUser } from "./services/types";

const getCourseOgImageUrl = (
  author: IUser,
  title: string,
  thumbnailUrl: string,
  theme = "light"
) => {
  return thumbnailUrl && thumbnailUrl !== ""
    ? thumbnailUrl
    : `https://imager.apps.vurilo.com?title=${encodeURIComponent(
        title
      )}&author=${encodeURIComponent(
        author.fullName ?? ""
      )}&image=${encodeURIComponent(author.imageUrl ?? "")}&theme=${theme}`;
};

export default getCourseOgImageUrl;
