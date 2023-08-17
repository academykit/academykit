import {
  Box,
  Button,
  Card,
  Container,
  createStyles,
  Grid,
  Group,
  Paper,
  ScrollArea,
  Text,
  Title,
  UnstyledButton,
} from '@mantine/core';
import { useMediaQuery } from '@mantine/hooks';
import {
  ILessonResultQuestionOption,
  ILessonStartQuestion,
} from '@utils/services/examService';
import SubmitResultHeader from './SubmitResultHeader';
import TextViewer from '@components/Ui/RichTextViewer';
import { IconSquareRoundedX } from '@tabler/icons';
import { IconCircleCheck } from '@tabler/icons-react';
import UserShortProfile from '@components/UserShortProfile';
import { IUser } from '@utils/services/types';
import moment from 'moment';
import { useState } from 'react';
import { useTranslation } from 'react-i18next';
const useStyle = createStyles((theme) => ({
  option: {
    '>label': {
      cursor: 'pointer',
    },
  },
  navigate: {
    display: 'flex',
    height: '50px',
    width: '50px',
    justifyContent: 'center',
    alignItems: 'center',
    cursor: 'pointer',
  },
  navigateWrapper: {
    maxHeight: '80vh',
    height: '100%',
    overflowY: 'auto',
    alignContent: 'start',
    justifyContent: 'start',
  },
  buttonNav: {
    display: 'flex',
    justifyContent: 'space-between',
    position: 'fixed',
    bottom: '0',
    right: '0',
    width: '100%',
    zIndex: 100,
  },
  active: {
    border: '2px solid ' + theme.colors[theme.primaryColor][1],
  },
  activeCircle: {
    outline: `4px solid ${theme.colors[theme.primaryColor][1]}`,
    transform: 'scale(1.08)',
  },
  unanswered: {
    backgroundColor: theme.colors.orange[6],
  },
  wrong: {
    border: `3px solid ${theme.colors.red[6]}`,
  },
  correct: {
    border: `3px solid ${theme.colors.green[6]}`,
  },

  correctCircle: {
    backgroundColor:
      theme.colorScheme == 'dark'
        ? theme.colors.green[8]
        : theme.colors.green[5],
  },
  errorCircle: {
    backgroundColor: theme.colors.red[5],
  },
  optionContainer: {
    order: 1,
  },
  examContainer: {
    order: 2,
  },
}));
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
  const { classes, theme, cx } = useStyle();
  const matches = useMediaQuery(`(min-width: ${theme.breakpoints.md}px)`);
  const [currentIndex, setCurrentIndex] = useState(0);
  const { t } = useTranslation();
  return (
    <Grid m={20}>
      <Grid.Col>
        <Paper radius={10} p={10}>
          <Group position="apart">
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
      <Grid.Col span={matches ? 9 : 12} className={classes.examContainer}>
        <ScrollArea>
          <Card p={4} my={10} shadow="lg" withBorder>
            <Box
              p={10}
              pb={20}
              sx={{
                flexDirection: 'column',
                width: '100%',
                justifyContent: 'start',
                alignContent: 'start',
              }}
            >
              <Group position="apart" align="center">
                <h3>{questions[currentIndex]?.name}</h3>
              </Group>
              {questions[currentIndex]?.description && (
                <TextViewer
                  key={currentIndex}
                  content={questions[currentIndex]?.description}
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
        <Card p={4} px={20} className={classes.buttonNav}>
          {currentIndex !== 0 ? (
            <Button
              onClick={() => {
                setCurrentIndex(currentIndex - 1);
              }}
            >
              {t('previous')}
            </Button>
          ) : (
            <div></div>
          )}
          <Text>
            {currentIndex + 1}/{questions.length}
          </Text>
          {currentIndex < questions.length - 1 ? (
            <Button
              onClick={() => {
                setCurrentIndex((currentIndex) => currentIndex + 1);
              }}
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
