import { Avatar, Flex, Grid, Tooltip, rem } from "@mantine/core";
import { IUser } from "@utils/services/types";
import { useTranslation } from "react-i18next";

interface TopThreeStudentsProps {
  students: IUser[];
  totalMarks: { marks: number }[];
}

const generateStudentTooltip = (
  student: IUser,
  marks: number
): React.ReactNode => (
  <Tooltip
    label={
      <>
        <div>{student ? `Name: ${student.fullName}` : "Name: -"}</div>
        <div>{`Marks: ${marks.toFixed(2)}`}</div>
      </>
    }
    position="top"
    color="rgba(112, 112, 112, 1)"
  >
    <Avatar
      variant="filled"
      radius={"100%"}
      size={90}
      src={student?.imageUrl}
    />
  </Tooltip>
);

const TopThreeStudents = ({ students, totalMarks }: TopThreeStudentsProps) => {
  const { t } = useTranslation();

  if (students.length === 0) {
    return <div>No students available</div>;
  }

  const [firstStudent, secondStudent, thirdStudent] = students;

  return (
    <>
      <Grid justify="center" align="center">
        {totalMarks[1]?.marks && (
          <Grid.Col span={3}>
            <Flex
              justify={"center"}
              direction="column"
              align="center"
              style={{ position: "relative" }}
            >
              {generateStudentTooltip(secondStudent, totalMarks[1]?.marks || 0)}
              <div style={{ marginTop: rem(10) }}>{t("rank_2")}</div>
            </Flex>
          </Grid.Col>
        )}

        <Grid.Col span={3} style={{ minHeight: rem(220) }}>
          <Flex
            justify={"center"}
            direction="column"
            align="center"
            style={{ position: "relative" }}
          >
            {generateStudentTooltip(firstStudent, totalMarks[0]?.marks || 0)}
            <div style={{ marginTop: rem(10) }}>{t("rank_1")}</div>
          </Flex>
        </Grid.Col>

        {totalMarks[2]?.marks && (
          <Grid.Col span={3}>
            <Flex
              justify={"center"}
              direction="column"
              align="center"
              style={{ position: "relative" }}
            >
              {generateStudentTooltip(thirdStudent, totalMarks[2]?.marks || 0)}
              <div style={{ marginTop: rem(10) }}>{t("rank_3")}</div>
            </Flex>
          </Grid.Col>
        )}
      </Grid>
    </>
  );
};

export default TopThreeStudents;
