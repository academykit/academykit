import TextViewer from '@components/Ui/RichTextViewer';
import UserShortProfile from '@components/UserShortProfile';
import {
  Box,
  Button,
  Card,
  Container,
  Grid,
  Group,
  Paper,
  ScrollArea,
  Text,
  Title,
  UnstyledButton,
  useMantineTheme,
} from '@mantine/core';
import { useMediaQuery } from '@mantine/hooks';
import { IconSquareRoundedX } from '@tabler/icons';
import { IconCircleCheck } from '@tabler/icons-react';
import {
  ILessonResultQuestionOption,
  ILessonStartQuestion,
} from '@utils/services/examService';
import { IUser } from '@utils/services/types';
import cx from 'clsx';
import moment from 'moment';
import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import classes from '../../styles/submittedResult.module.css';
import SubmitResultHeader from './SubmitResultHeader';

const SubmittedResultDetails = ({
  questions,
  duration,
  marks,
  name,
  totalMarks,
  submissionDate,
  user,
}: {
  questions: ILessonStartQuestion<ILessonResultQuestionOption>[];
  name: string;
  duration: string;
  marks: number;
  totalDuration: string;
  totalMarks: number;
  submissionDate: string;
  user: IUser;
}) => {
  const theme = useMantineTheme();
  const matches = useMediaQuery(`(min-width: ${theme.breakpoints.md}px)`);
  const [currentIndex, setCurrentIndex] = useState(0);
  const { t } = useTranslation();

  return (
    <Grid m={20}>
      <Grid.Col>
        <Paper radius={10} p={10}>
          <Group justify="space-between">
            <div>
              <Title>{name}</Title>
              <Text>{moment(submissionDate + 'Z').fromNow()}</Text>
            </div>
            <UserShortProfile user={user}></UserShortProfile>
            <SubmitResultHeader
              duration={duration}
              marks={marks}
              totalMarks={totalMarks}
            />
          </Group>
        </Paper>
      </Grid.Col>
      <Grid.Col
        span={matches ? 9 : 12}
        style={{ maxWidth: '100%' }}
        className={classes.examContainer}
      >
        <ScrollArea>
          <Card p={4} my={10} shadow="lg" withBorder>
            <Box
              p={10}
              pb={20}
              style={{
                flexDirection: 'column',
                width: '100%',
                justifyContent: 'start',
                alignContent: 'start',
              }}
            >
              <Group justify="space-between" ta="center">
                <h3>{questions[currentIndex]?.name}</h3>
              </Group>
              {questions[currentIndex]?.description && (
                <TextViewer
                  key={currentIndex}
                  content={questions[currentIndex]?.description}
                  styles={{ wordBreak: 'break-all' }}
                />
              )}
            </Box>
            <Container fluid className={classes.option}>
              {questions[currentIndex]?.questionOptions?.map((x) => (
                <Card
                  key={x.id}
                  className={cx({
                    [classes.active]: x.isSelected,
                  })}
                  id={x.id}
                  shadow={'lg'}
                  my={5}
                >
                  <Grid justify="start" align="center">
                    <Grid.Col span={11}>
                      <TextViewer
                        key={x.id}
                        styles={{
                          root: {
                            border: 'none',
                            background: 'transparent',
                          },
                        }}
                        content={x.value}
                      />
                    </Grid.Col>
                    {/* showing icon if correct answer was selected */}
                    {x.isCorrect && x.isSelected && (
                      <IconCircleCheck
                        size={36}
                        color={theme.colors.green[6]}
                      />
                    )}

                    {/* showing icon if right answer was not selected */}
                    {x.isCorrect && !x.isSelected && (
                      <IconCircleCheck
                        size={36}
                        color={theme.colors.green[6]}
                      />
                    )}

                    {/* shoing icon if wrong answer was selected */}
                    {!x.isCorrect && x.isSelected && (
                      <IconSquareRoundedX
                        size={36}
                        color={theme.colors.red[6]}
                      />
                    )}
                  </Grid>
                </Card>
              ))}
            </Container>
          </Card>

          {questions[currentIndex].hints && (
            <Card p={10} my={10} shadow="lg" withBorder>
              <Text size={'lg'} mb={10}>
                Hints:
              </Text>
              <TextViewer
                key={questions[currentIndex].id}
                styles={{
                  root: {
                    border: 'none',
                    background: 'transparent',
                  },
                }}
                content={questions[currentIndex].hints}
              />
            </Card>
          )}
        </ScrollArea>
        <Card p={20} className={classes.buttonNav}>
          {currentIndex !== 0 ? (
            <Button
              onClick={() => {
                setCurrentIndex(currentIndex - 1);
              }}
              w={100}
            >
              {t('previous')}
            </Button>
          ) : (
            <div></div>
          )}
          <Text my={5}>
            {currentIndex + 1}/{questions.length}
          </Text>
          {currentIndex < questions.length - 1 ? (
            <Button
              onClick={() => {
                setCurrentIndex((currentIndex) => currentIndex + 1);
              }}
              w={100}
            >
              {t('next')}
            </Button>
          ) : (
            <div></div>
          )}
        </Card>
      </Grid.Col>
      <Grid.Col
        span={matches ? 3 : 12}
        m={0}
        className={classes.optionContainer}
      >
        <Group p={10} className={classes.navigateWrapper}>
          {questions.map((x, i) => (
            <UnstyledButton
              key={i}
              onClick={() => {
                setCurrentIndex(i);
              }}
              style={{
                outline: 'none',
                border: 'none',
                backgroundColor: 'none',
              }}
            >
              <Card
                className={cx(classes.navigate, {
                  [classes.activeCircle]: currentIndex === i,
                  [classes.correctCircle]: x.isCorrect,
                  [classes.errorCircle]:
                    x.questionOptions.filter((x) => x.isSelected).length >= 1 &&
                    !x.isCorrect,
                  [classes.unanswered]:
                    x.questionOptions.filter((x) => x.isSelected).length === 0,
                })}
                radius={10000}
              >
                {i + 1}
              </Card>
            </UnstyledButton>
          ))}
        </Group>
      </Grid.Col>
    </Grid>
  );
};

export default SubmittedResultDetails;
