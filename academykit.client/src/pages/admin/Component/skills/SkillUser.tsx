import { Avatar, Flex, Paper, Text } from "@mantine/core";
import { IUser } from "@utils/services/types";

const SkillUser = ({ user }: { user: IUser }) => {
  return (
    <Paper p={"sm"} withBorder my={10}>
      <Flex gap={10} align={"center"}>
        <Avatar src={user.imageUrl} alt="user name" />
        <Text>{user.fullName}</Text>
      </Flex>
    </Paper>
  );
};

export default SkillUser;
