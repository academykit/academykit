import { Avatar, Flex, Grid, Tooltip, rem } from "@mantine/core";
import { IUser } from "@utils/services/types";
import { useTranslation } from "react-i18next";

interface TopThreePerformersProps {
  students: IUser[];
}

const generateStudentTooltip = (student: IUser): React.ReactNode => (
  <Tooltip
    label={
      <>
        <div>{student ? `Name: ${student.fullName}` : "Name: -"}</div>
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

const TopThreePerformers = ({ students }: TopThreePerformersProps) => {
  const { t } = useTranslation();
  const [firstStudent, secondStudent, thirdStudent] = students;

  if (students.length === 0) {
    return <div>No students available</div>;
  }

  return (
    <>
      <Grid justify="center" align="center">
        {students[1]?.fullName && (
          <Grid.Col span={3}>
            <Flex
              justify={"center"}
              direction="column"
              align="center"
              style={{ position: "relative" }}
            >
              {generateStudentTooltip(secondStudent)}
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
            {generateStudentTooltip(firstStudent)}
            <div style={{ marginTop: rem(10) }}>{t("rank_1")}</div>
          </Flex>
        </Grid.Col>

        {students[2]?.fullName && (
          <Grid.Col span={3}>
            <Flex
              justify={"center"}
              direction="column"
              align="center"
              style={{ position: "relative" }}
            >
              {generateStudentTooltip(thirdStudent)}
              <div style={{ marginTop: rem(10) }}>{t("rank_3")}</div>
            </Flex>
          </Grid.Col>
        )}
      </Grid>
    </>
  );
};

export default TopThreePerformers;
