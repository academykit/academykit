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
import { DATE_FORMAT } from "@utils/constants";
import RoutePath from "@utils/routeConstants";
import { useMyResult } from "@utils/services/examService";
import axios from "axios";
import moment from "moment";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";

const UserResults = ({
  lessonId,
  studentId,
}: {
  lessonId: string;
  studentId: string;
}) => {
  const theme = useMantineTheme();
  const { t } = useTranslation();
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
        return <Box> {t("no_result")}</Box>;
      } else {
        return <div>{t("something_wrong")}</div>;
      }
    }
  }
  if (result.data?.attemptCount === 0) {
    return (
      <Title my={10} size={"sm"}>
        {t("no_previous_attempt")}
      </Title>
    );
  }
  return (
    <>
      <Title mt={20}> {t("previous_result")}</Title>
      <Table
        sx={(theme) => ({
          ...theme.defaultGradient,
        })}
        w={"100%"}
        striped
      >
        <thead>
          <tr>
            <th>{t("obtained")}</th>
            <th>{t("submission_date")}</th>
            <th>{t("completed_duration")}</th>
            <th>{t("actions")}</th>
          </tr>
        </thead>
        <tbody>
          {result.data?.questionSetSubmissions?.map((r) => (
            <tr key={r.questionSetSubmissionId}>
              <td>{r.obtainedMarks}</td>
              <td>{moment(r.submissionDate).format(DATE_FORMAT)}</td>
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
