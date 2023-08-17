import TextViewer from '@components/Ui/RichTextViewer';
import {
  Paper,
  Flex,
  Group,
  Box,
  Select,
  Text,
  Button,
  Collapse,
} from '@mantine/core';
import { IconChevronDown, IconChevronUp, IconDragDrop } from '@tabler/icons';
import { QuestionType, ReadableEnum } from '@utils/enums';
import { QuestionSetQuestions } from '@utils/services/questionService';
import { useTranslation } from 'react-i18next';
import { useDisclosure } from '@mantine/hooks';

const PreviewQuestionCard = ({
  question,
}: {
  question: QuestionSetQuestions;
}) => {
  const { t } = useTranslation();
  const [opened, { toggle }] = useDisclosure(false);
  const getQuestionType = () => {
    return Object.entries(QuestionType)
      .splice(0, Object.entries(QuestionType).length / 2)
      .map(([key, value]) => ({
        value: key,
        label:
          ReadableEnum[value as keyof typeof ReadableEnum] ?? value.toString(),
      }));
  };

  return (
    <>
      <Paper p={10} withBorder mb={25}>
        <Flex justify={'space-between'}>
          <Text size={'lg'} truncate w={'92%'}>
            {question.name}
          </Text>
          <Group>
            <IconDragDrop />

            <Button type="button" compact variant="subtle" onClick={toggle}>
              {opened ? <IconChevronUp /> : <IconChevronDown />}
            </Button>
          </Group>
        </Flex>

        <Collapse in={opened}>
          {question.description && (
            <Box my={10}>
              <Text>{t('description')}</Text>
              <TextViewer key={question.id} content={question.description} />
            </Box>
          )}

          {question.hints && (
            <Box my={10}>
              <Text size={'sm'}>{t('hint')}</Text>
              <TextViewer key={question.id} content={question.hints} />
            </Box>
          )}

          <Select
            mt={20}
            placeholder={t('question_type') as string}
            label={t('question_type')}
            data={getQuestionType()}
            value={question.type.toString()}
            onChange={() => {}}
            disabled
          />

          <Box my={20}>
            <>
              <Text>{t('options')}</Text>
              {question.questionOptions.map((option) => (
                <Group my={10} key={option.id} id="hehe">
                  <div style={{ width: '100%' }}>
                    <TextViewer
                      content={option.option}
                      styles={{ root: { flexGrow: 1 } }}
                    ></TextViewer>
                  </div>
                </Group>
              ))}
            </>
          </Box>
        </Collapse>
      </Paper>
    </>
  );
};

export default PreviewQuestionCard;
