import {
  Box,
  Card,
  Container,
  createStyles,
  Grid,
  Group,
  Paper,
  ScrollArea,
  Text,
  Title,
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
    backgroundColor: theme.colors.green[5],
  },
  errorCircle: {
    backgroundColor: theme.colors.red[5],
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
      <Grid.Col span={matches ? 9 : 12}>
        <ScrollArea>
          {questions.map((question, i) => (
            <Card p={4} key={i} my={10} shadow="lg" withBorder>
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
                  <h3>{question?.name}</h3>
                </Group>
                {question?.description && (
                  <TextViewer content={question?.description} />
                )}
              </Box>
              <Container fluid className={classes.option}>
                {question?.questionOptions?.map((x) => (
                  <Card
                    key={x.id}
                    className={cx({
                      [classes.active]: x.isSelected,
                    })}
                    id={x.id}
                    shadow={'lg'}
                    my={5}
                  >
                    <Grid justify="space-between" align="center">
                      {x.isCorrect && x.isSelected && (
                        <IconCircleCheck
                          size={36}
                          color={theme.colors.green[6]}
                        />
                      )}
                      {!x.isCorrect && x.isSelected && (
                        <IconSquareRoundedX
                          size={36}
                          color={theme.colors.red[6]}
                        />
                      )}
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
                    </Grid>
                  </Card>
                ))}
              </Container>
            </Card>
          ))}
        </ScrollArea>
      </Grid.Col>
    </Grid>
  );
};

export default SubmittedResultDetails;
