import {
  Button,
  Center,
  Group,
  Paper,
  Table,
  Box,
  Loader,
} from "@mantine/core";
import { showNotification } from "@mantine/notifications";
import { LessonType } from "@utils/enums";
import errorType from "@utils/services/axiosError";
import { exportFeedback } from "@utils/services/feedbackService";
import { useGetLessonStatisticsDetails } from "@utils/services/manageCourseService";
import { useState } from "react";
import { useParams } from "react-router-dom";
import CourseLessonDetails from "./Components/CourseLessonDetails";
import { useTranslation } from "react-i18next";

const LessonDetails = () => {
  const { id, lessonId } = useParams();
  const { t } = useTranslation();

  const lessonDetails = useGetLessonStatisticsDetails(
    id as string,
    lessonId as string
  );
  const [loading, setLoading] = useState(false);

  if (lessonDetails.data && lessonDetails.data?.totalCount < 1) {
    return <Box>{t("no_enrolled_student_found")}</Box>;
  }

  if (lessonDetails.isLoading) return <Loader />;

  if (lessonDetails.error) throw lessonDetails.error;

  const handleExport = async () => {
    setLoading(true);
    try {
      const res = await exportFeedback(lessonId as string);

      var element = document.createElement("a");
      setLoading(false);

      element.setAttribute(
        "href",
        "data:text/plain;charset=utf-8," +
          encodeURIComponent(res.data as string)
      );
      element.setAttribute("download", "Feedback-" + lessonId + ".csv");
      document.body.appendChild(element);
      element.click();
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        title: t("error"),
        color: "red",
      });
    }
    setLoading(false);
  };

  return (
    <>
      {lessonDetails.data?.items[0].lessonType === LessonType.Feedback && (
        <Group position="right" my="md">
          <Button onClick={handleExport} loading={loading}>
            {t("export")}
          </Button>
        </Group>
      )}
      <Paper>
        <Table striped withBorder>
          <thead>
            <tr>
              <th>{t("trainees")}</th>
              <th>
                <Center>{t("status")}</Center>
              </th>

              <th>{t("actions")}</th>
            </tr>
          </thead>
          <tbody>
            {lessonDetails.data?.items.map((x) => (
              <CourseLessonDetails element={x} key={x.lessonId} />
            ))}
          </tbody>
        </Table>
      </Paper>
    </>
  );
};

export default LessonDetails;
