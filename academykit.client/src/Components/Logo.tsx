import { Center, Image } from "@mantine/core";
import { checkValidUrl } from "@utils/checkValidUrl";
import { Link } from "react-router-dom";
import CompanyLogo from "./Icons/Company";

const Logo = ({
  url,
  height,
  width,
}: {
  url?: string;
  height: number;
  width: number;
}) => {
  return (
    <Center>
      <Link to={"/"}>
        {checkValidUrl(url ?? "") ? (
          <Image
            height={height}
            width={width}
            src={url}
            alt="logo"
            fit="contain"
          />
        ) : (
          <CompanyLogo />
        )}
      </Link>
    </Center>
  );
};

export default Logo;
