import {
  Anchor,
  Avatar,
  Box,
  Flex,
  MantineNumberSize,
  SystemProp,
  Text,
  Title,
} from "@mantine/core";
import { PoolRole, UserRole } from "@utils/enums";
import { getInitials } from "@utils/getInitialName";
import { IUser } from "@utils/services/types";
import { CSSProperties, FC } from "react";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";

type Props = {
  user: IUser;
  size?: MantineNumberSize | undefined;
  direction?: SystemProp<CSSProperties["flexDirection"]>;
  page?: string;
};

const UserShortProfile: FC<Props> = ({
  user: { email, fullName, id, imageUrl, mobileNumber, role },
  size = "xl",
  direction = undefined,
  page = "Pool",
}) => {
  const { t } = useTranslation();
  return (
    <Anchor
      component={Link}
      to={`/userProfile/${id}`}
      sx={{ textDecoration: "none" }}
    >
      <Flex direction={direction} gap={"md"} sx={{ alignItems: "center" }}>
        <Avatar my={3} src={imageUrl} color="cyan" radius={10000} size={size}>
          {!imageUrl && getInitials(fullName ?? "")}
        </Avatar>
        <Box>
          <Text size={size} weight="bolder">
            {fullName}
          </Text>
          <Text size={"sm"} color={"dimmed"}>
            {page === "Pool" ? t(`${PoolRole[role]}`) : t(`${UserRole[role]}}`)}
          </Text>
        </Box>
      </Flex>
    </Anchor>
  );
};

export default UserShortProfile;
