import { IAuthContext } from '@context/AuthProvider';
import { Avatar, Flex, Paper, Text } from '@mantine/core';

interface IProps {
  user: IAuthContext | null;
}

const UserChatBox = ({ user }: IProps) => {
  return (
    <>
      <Flex direction={'column'} gap={10} mb={30} mr={15}>
        <Flex align={'center'} justify={'flex-end'} gap={10}>
          <Avatar radius="xl" size={'sm'} src={user?.auth?.imageUrl} />
          <Text size={'sm'} fw="bolder">
            {user?.auth?.fullName}
          </Text>
        </Flex>

        <Flex align={'center'} justify={'flex-end'}>
          <Paper
            p={'md'}
            withBorder
            w={'fit-content'}
            maw={'80vw'}
            style={{ wordBreak: 'break-word' }}
          >
            HEllo, create an examOkay, Im on it. It will be done
          </Paper>
        </Flex>
      </Flex>
    </>
  );
};

export default UserChatBox;
