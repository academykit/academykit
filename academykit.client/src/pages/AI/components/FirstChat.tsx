import { Avatar, Flex, Title } from "@mantine/core";
import { useTranslation } from "react-i18next";

const FirstChat = () => {
  const { t } = useTranslation();

  return (
    <Flex
      w={"100%"}
      direction={"column"}
      justify={"center"}
      align={"center"}
      gap={10}
      pos={"absolute"}
      top={90}
      left={"50%"}
      style={{ transform: "translateX(-50%)" }}
    >
      <Avatar radius={"100%"} size={"xl"} />
      <Title ta={"center"}>{t("how_can_i_help")}</Title>
    </Flex>
  );
};

export default FirstChat;
