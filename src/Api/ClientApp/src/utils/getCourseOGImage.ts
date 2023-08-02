import { IUser } from './services/types';

const getCourseOgImageUrl = (
  author: IUser,
  title: string,
  thumbnailUrl: string,
  theme = 'light'
) => {
  console.log(
    `?title=${encodeURIComponent(title)}&author=${encodeURIComponent(
      author.fullName ?? ''
    )}&image=${encodeURIComponent(author.imageUrl ?? '')}&theme=${theme}`
  );
  return thumbnailUrl && thumbnailUrl !== ''
    ? thumbnailUrl
    : `https://imager.apps.vurilo.com?title=${encodeURIComponent(
        title
      )}&author=${encodeURIComponent(
        author.fullName ?? ''
      )}&image=${encodeURIComponent(author.imageUrl ?? '')}&theme=${theme}`;
};

export default getCourseOgImageUrl;
