import {
  ActionIcon,
  Box,
  Loader,
  MantineProvider,
  Table,
  Title,
  useMantineTheme,
} from "@mantine/core";
import { IconEye } from "@tabler/icons";
import RoutePath from "@utils/routeConstants";
import { useMyResult } from "@utils/services/examService";
import axios from "axios";
import moment from "moment";
import { Link } from "react-router-dom";

const UserResults = ({
  lessonId,
  studentId,
}: {
  lessonId: string;
  studentId: string;
}) => {
  const theme = useMantineTheme();
  const result = useMyResult(lessonId, studentId);
  if (result.isLoading) {
    return <Loader />;
  }
  if (result.isError) {
    if (axios.isAxiosError(result.error)) {
      if (result.error.response?.data) {
        // @ts-ignore
        return <Box>{result.error.response?.data?.message}</Box>;
      } else if (result.error.response?.status === 404) {
        return <Box> No Result has been found</Box>;
      } else {
        return <div>Something went wrong</div>;
      }
    }
  }
  if (result.data?.attemptCount === 0) {
    return (
      <Title my={10} size={"sm"}>
        No Previous Attempt available
      </Title>
    );
  }
  return (
    <>
      <Title mt={20}> Previous Results</Title>
      <Table
        sx={(theme) => ({
          ...theme.defaultGradient,
        })}
        w={"100%"}
        striped
      >
        <thead>
          <tr>
            <th>Obtained</th>
            <th>Submission Date</th>
            <th>Completed Duration</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {result.data?.questionSetSubmissions?.map((r) => (
            <tr key={r.questionSetSubmissionId}>
              <td>{r.obtainedMarks}</td>
              <td>{moment(r.submissionDate).format(theme.dateFormat)}</td>
              <td>{r.completeDuration}</td>
              <td>
                <ActionIcon
                  component={Link}
                  to={
                    RoutePath.exam.resultOne(
                      lessonId,
                      r.questionSetSubmissionId
                    ).route
                  }
                >
                  <IconEye />
                </ActionIcon>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>
    </>
  );
};

export default UserResults;
