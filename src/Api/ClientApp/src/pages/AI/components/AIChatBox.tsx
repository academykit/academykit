import { Avatar, Flex, Loader, Paper, Text } from '@mantine/core';

const AIChatBox = () => {
  const isLoading = false;
  return (
    <>
      <Flex direction={'column'} gap={10} mb={30}>
        <Flex align={'center'} gap={10}>
          <Avatar radius="xl" size={'sm'} />
          <Text size={'sm'} fw="bolder">
            Vurilo AI
          </Text>
        </Flex>

        {isLoading ? (
          <Loader color="cyan" type="dots" size={20} />
        ) : (
          <>
            <Paper
              p={'md'}
              withBorder
              w={'fit-content'}
              maw={'80vw'}
              style={{ wordBreak: 'break-word' }}
            >
              Okay, Im on it.
            </Paper>

            {/* <Flex gap={5} align={'center'}>
              <ActionIcon variant="transparent" c={'gray'}>
                <IconThumbUp size={18} />
              </ActionIcon>
              <ActionIcon variant="transparent" c={'gray'}>
                <IconThumbDown size={18} />
              </ActionIcon>
            </Flex> */}
          </>
        )}
      </Flex>
    </>
  );
};

export default AIChatBox;
