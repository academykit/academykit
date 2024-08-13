import useAuth from "@hooks/useAuth";
import { Button, Group, Text, Title } from "@mantine/core";
import { showNotification } from "@mantine/notifications";
import { DATE_FORMAT } from "@utils/constants";
import { UserRole } from "@utils/enums";
import errorType from "@utils/services/axiosError";
import { usePostAttendance } from "@utils/services/physicalTrainingService";
import moment from "moment";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";

const PhysicalTrainingDetail = ({
  name,
  id,
  hasAttended,
  startDate,
  lessonSlug,
  isTrainee,
}: {
  name: string;
  id: string;
  hasAttended: boolean | null;
  startDate: string;
  lessonSlug: string;
  isTrainee: boolean;
}) => {
  const { id: slug } = useParams();
  const { t } = useTranslation();
  const attendance = usePostAttendance(slug as string, lessonSlug);
  const startTime = moment(startDate).format("HH:mm A");
  const user = useAuth();

  const updatedTime = moment(startTime, "hh:mm A")
    .add(5, "hours")
    .add(45, "minutes")
    .format("hh:mm A");

  const handleAttendance = async () => {
    try {
      const response = await attendance.mutateAsync({ identity: id });
      showNotification({
        message: response.data.message,
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };

  return (
    <Group style={{ flexDirection: "column" }}>
      <Title>{name}</Title>
      <Text>
        {t("start_date")}: {moment(startDate).format(DATE_FORMAT)}
      </Text>
      <Text>
        {t("start_time")}: {updatedTime}
      </Text>
      {/* Super admin,  admin and trainer of that lesson cannot mark as attend */}
      {!hasAttended
        ? ((user?.auth?.role === UserRole.Trainer && isTrainee) ||
            isTrainee) && (
            <Button
              onClick={() => handleAttendance()}
              loading={attendance.isPending || attendance.isSuccess}
            >
              {t("mark_as_attended")}
            </Button>
          )
        : user?.auth?.role !== UserRole.Admin &&
          user?.auth?.role !== UserRole.SuperAdmin && (
            <Text>{t("attended")}</Text>
          )}
    </Group>
  );
};

export default PhysicalTrainingDetail;
