import Breadcrumb from '@components/Ui/BreadCrumb';
import { Box, Flex, Group, Paper, Title, Text } from '@mantine/core';
import { IconDragDrop } from '@tabler/icons';
import TextViewer from '@components/Ui/RichTextViewer';
import { useTranslation } from 'react-i18next';

const PreviewQuestion = () => {
  const { t } = useTranslation();

  return (
    <div>
      <Breadcrumb hide={3} />
      <Paper p={10} withBorder>
        <Flex justify={'space-between'}>
          <Title truncate>{'Title'}</Title>
          <Group>
            <IconDragDrop />
          </Group>
        </Flex>

        {'data.description' && (
          <Box my={10}>
            <Text>{t('description')}</Text>
            <TextViewer key={'data.id'} content={'data.description'} />
          </Box>
        )}
        {'data.hint' && (
          <Box my={10}>
            <Text size={'sm'}>{t('hint')}</Text>
            <TextViewer key={'data.id'} content={'data.hint'} />
          </Box>
        )}
      </Paper>
    </div>
  );
};

export default PreviewQuestion;
