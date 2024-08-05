import { IUser } from './services/types';

const getCourseOgImageUrl = ({
  author,
  title,
  thumbnailUrl,
  theme = 'light',
  companyName,
  companyLogo,
}: {
  author: IUser;
  title: string;
  thumbnailUrl: string;
  theme?: string;
  companyName?: string;
  companyLogo?: string;
}) => {
  return thumbnailUrl && thumbnailUrl !== ''
    ? thumbnailUrl
    : `/api/utility/ogimage?title=${encodeURIComponent(
        title
      )}&author=${encodeURIComponent(
        author.fullName ?? ''
      )}&image=${encodeURIComponent(
        author.imageUrl ?? ''
      )}&theme=${theme}&company=${encodeURIComponent(
        companyName ?? ''
      )}&logo=${encodeURIComponent(companyLogo ?? '')}`;
};

export default getCourseOgImageUrl;
