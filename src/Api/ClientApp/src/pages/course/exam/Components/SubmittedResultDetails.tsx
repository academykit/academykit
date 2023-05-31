import useCustomLayout from "@context/LayoutProvider";
import {
  Box,
  Button,
  Card,
  Container,
  createStyles,
  Grid,
  Group,
  Text,
  Title,
} from "@mantine/core";
import { useMediaQuery } from "@mantine/hooks";
import RichTextEditor from "@mantine/rte";
import {
  ILessonResultQuestionOption,
  ILessonStartQuestion,
} from "@utils/services/examService";
import { useEffect, useState } from "react";
import SubmitResultHeader from "./SubmitResultHeader";
import { useTranslation } from "react-i18next";
const useStyle = createStyles((theme) => ({
  option: {
    padding: 20,
    width: "100%",
    justifyContent: "start",
    alignItems: "start",
    border: "1px solid gray",
    ">label": {
      cursor: "pointer",
    },
  },
  navigate: {
    display: "flex",
    height: "50px",
    width: "50px",
    justifyContent: "center",
    alignItems: "center",
    cursor: "pointer",
  },
  navigateWrapper: {
    border: "1px solid grey",
    maxHeight: "80vh",
    height: "100%",
    overflowY: "auto",
    alignContent: "start",
    justifyContent: "start",
  },
  buttonNav: {
    display: "flex",
    justifyContent: "space-between",
    position: "fixed",
    bottom: "0",
    right: "0",
    width: "100%",
    zIndex: 100,
  },
  active: {
    backgroundColor: theme.colors[theme.primaryColor][1],
  },
  activeCircle: {
    outline: `4px solid ${theme.colors[theme.primaryColor][1]}`,
    transform: "scale(1.08)",
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
  totalDuration,
  totalMarks,
}: {
  questions: ILessonStartQuestion<ILessonResultQuestionOption>[];
  name: string;
  duration: string;
  marks: number;
  totalDuration: string;
  totalMarks: number;
}) => {
  const { classes, theme, cx } = useStyle();
  const matches = useMediaQuery(`(min-width: ${theme.breakpoints.md}px)`);
  const { t } = useTranslation();
  const [currentIndex, setCurrentIndex] = useState(0);
  const customLayout = useCustomLayout();

  useEffect(() => {
    customLayout.setExamPage && customLayout.setExamPage(true);
    customLayout.setExamPageAction &&
      customLayout.setExamPageAction(
        <SubmitResultHeader
          duration={duration}
          marks={marks}
          totalMarks={totalMarks}
        />
      );
    customLayout.setExamPageTitle &&
      customLayout.setExamPageTitle(<Title>{name}</Title>);
    return () => {
      customLayout.setExamPage && customLayout.setExamPage(false);
    };
  }, [customLayout.examPage]);

  return (
    <Grid m={20}>
      <Grid.Col span={matches ? 9 : 12}>
        <Box
          sx={{
            flexDirection: "column",
            overflow: "auto",
          }}
        >
          <Box
            p={10}
            pb={20}
            sx={{
              flexDirection: "column",
              width: "100%",
              justifyContent: "start",
              alignContent: "start",
            }}
          >
            <Title mb={20}>{questions[currentIndex]?.name}</Title>
            {questions[currentIndex]?.description && (
              <RichTextEditor
                readOnly
                value={questions[currentIndex]?.description}
              />
            )}
          </Box>
          <Container className={classes.option}>
            {questions[currentIndex]?.questionOptions?.map((x) => (
              <Card
                className={cx({
                  [classes.active]: x.isSelected,
                  [classes.wrong]: !x.isCorrect && x.isSelected,
                  [classes.correct]: x.isCorrect,
                })}
                id={x.id}
                shadow={"lg"}
                radius={10}
                my={10}
              >
                <RichTextEditor
                  styles={{
                    root: {
                      border: "none",
                      backgroundColor: "transparent",
                    },
                  }}
                  readOnly
                  value={x.value}
                />
              </Card>
            ))}
          </Container>
        </Box>
        <Card p={4} px={20} className={classes.buttonNav}>
          {currentIndex !== 0 ? (
            <Button
              onClick={() => {
                setCurrentIndex(currentIndex - 1);
              }}
            >
              {t("previous")}
            </Button>
          ) : (
            <div></div>
          )}
          {/* @ts-ignore */}
          <button style={{ display: "none" }}></button>
          <Text>
            {currentIndex + 1}/{questions.length}
          </Text>

          {currentIndex < questions.length - 1 ? (
            <Button
              onClick={() => {
                setCurrentIndex((currentIndex) => currentIndex + 1);
              }}
            >
              {t("next")}
            </Button>
          ) : (
            <></>
          )}
        </Card>
      </Grid.Col>
      <Grid.Col span={matches ? 3 : 12} m={0}>
        <Group p={10} className={classes.navigateWrapper}>
          {questions.map((x, i) => (
            <div
              key={i}
              onClick={() => {
                setCurrentIndex(i);
              }}
              style={{
                outline: "none",
                border: "none",
                backgroundColor: "none",
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
            </div>
          ))}
        </Group>
      </Grid.Col>
    </Grid>
  );
};

export default SubmittedResultDetails;
