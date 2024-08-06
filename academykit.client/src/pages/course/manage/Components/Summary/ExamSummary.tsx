import { Box, Grid, Paper, SimpleGrid } from "@mantine/core";
import { useGetExamSummary } from "@utils/services/examService";
import { useTranslation } from "react-i18next";
import Ranking from "../Ranking";
import SummaryCard from "../SummaryCard";
import TopThreeStudents from "./TopThreeStudents";

interface ExamSummaryProps {
  courseIdentity: string;
  lessonId: string;
}

const ExamSummary = ({ courseIdentity, lessonId }: ExamSummaryProps) => {
  const { t } = useTranslation();
  const { data, isLoading, isError } = useGetExamSummary(
    courseIdentity,
    lessonId
  );

  if (isLoading) {
    return <div>Loading...</div>;
  }

  if (isError) {
    return <div>Error fetching data</div>;
  }

  const {
    examStatus,
    weekStudents,
    topStudents,
    mostWrongAnsQues,
    totalMarks,
  } = data;

  const [firstStudent, secondStudent, thirdStudent, ...otherStudents] =
    topStudents;

  return (
    <>
      <Box>
        <SimpleGrid
          cols={{ base: 1, sm: 2, lg: 5 }}
          spacing={{ base: 5, sm: "xl" }}
          verticalSpacing={{ base: "md", sm: "xl" }}
          mb={15}
        >
          <SummaryCard
            title={t("total_attendees")}
            count={examStatus.totalAttend}
          />
          <SummaryCard
            title={t("total_passed")}
            count={examStatus.passStudents}
          />
          <SummaryCard
            title={t("total_failed")}
            count={examStatus.failStudents}
          />
          <SummaryCard
            title={t("average")}
            count={Number(examStatus.averageMarks.toPrecision(3))}
          />
        </SimpleGrid>

        <Grid>
          <Grid.Col
            span={6}
            order={{ base: 2, sm: 2, lg: 1 }}
            miw={{ base: "100%", sm: "100%", md: "50%", lg: "50%" }}
          >
            <Box h={"100%"}>
              <Ranking
                title="most_wrong_answered_questions"
                data={mostWrongAnsQues}
              />
              <Ranking title="weak_students" data={weekStudents.reverse()} />
            </Box>
          </Grid.Col>

          <Grid.Col
            span={6}
            order={{ base: 1, sm: 1, lg: 2 }}
            miw={{ base: "100%", sm: "100%", md: "50%", lg: "50%" }}
          >
            <TopThreeStudents
              students={[firstStudent, secondStudent, thirdStudent]}
              totalMarks={totalMarks}
            />

            <Paper p={"md"} mb={10}>
              <ol start={4} style={{ paddingLeft: "18px", margin: 0 }}>
                {otherStudents.map((student) => (
                  <li key={student.id}>{student.fullName}</li>
                ))}
              </ol>
            </Paper>
          </Grid.Col>
        </Grid>
      </Box>
    </>
  );
};

export default ExamSummary;
