import CustomTextFieldWithAutoFocus from "@components/Ui/CustomTextFieldWithAutoFocus";
import useFormErrorHooks from "@hooks/useFormErrorHooks";
import {
  ActionIcon,
  Box,
  Button,
  Container,
  Flex,
  Grid,
  Image,
  SimpleGrid,
  Text,
  Title,
  Tooltip,
} from "@mantine/core";
import { DatePickerInput } from "@mantine/dates";
import { useForm, yupResolver } from "@mantine/form";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconCalendar, IconDownload, IconEye } from "@tabler/icons-react";
import downloadImage from "@utils/downloadImage";
import errorType from "@utils/services/axiosError";
import {
  useAddCertificate,
  useGetCertificateDetails,
  useGetSignature,
} from "@utils/services/courseService";
import moment from "moment";
import { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";
import * as Yup from "yup";
import CreateSignature from "./Components/Signature/CreateSignature";

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    title: Yup.string()
      .required(t("course_title_required") as string)
      .max(80, t("course_title_less_than_80") as string),
    eventStartDate: Yup.date()
      .required(t("event_start_date_required") as string)
      .typeError(t("event_start_date_required") as string),
    eventEndDate: Yup.date()
      .min(Yup.ref("eventStartDate"), "End Date Needs to be after Start Date")
      .required(t("event_end_date_required") as string)
      .typeError(t("event_end_date_required") as string),
  });
};

const Certificate = () => {
  const params = useParams();
  const getSignature = useGetSignature(params.id as string);
  const addCertificate = useAddCertificate(params.id as string);
  const getCertificateDetails = useGetCertificateDetails(params.id as string);
  const data = getCertificateDetails.data?.data;
  const { t } = useTranslation();

  const form = useForm({
    validate: yupResolver(schema()),
    initialValues: {
      title: "",
      eventStartDate: new Date(),
      eventEndDate: new Date(),
    },
  });
  useFormErrorHooks(form);

  useEffect(() => {
    if (
      getCertificateDetails.isSuccess &&
      getCertificateDetails.data?.status === 200
    ) {
      const data = getCertificateDetails.data;

      form.setValues({
        title: data.data.title,
        eventEndDate: moment(data.data.eventEndDate + "Z")
          .local()
          .toDate(),
        eventStartDate: moment(data.data.eventStartDate + "Z")
          .local()
          .toDate(),
      });
    }
  }, [getCertificateDetails.isSuccess]);

  const [addSignatureForm, setAddSignatureForm] = useToggle();
  const handleSubmit = async (values: any) => {
    const isEditing = !(
      getCertificateDetails.isError || !getCertificateDetails.data
    );
    const editData = {
      ...values,
      id: getCertificateDetails.data?.data?.id ?? "",
    };

    try {
      await addCertificate.mutateAsync({
        data: isEditing ? editData : values,
        id: params.id as string,
      });
      showNotification({
        title: t("success"),
        message: `${t("certificate_details")} ${
          isEditing ? t("updated") : t("added")
        } ${t("successfully")}`,
      });
    } catch (error) {
      const err = errorType(error);
      showNotification({
        title: t("error"),
        message: err,
        color: "red",
      });
    }
  };

  return (
    <>
      <Title>{t("certificates")}</Title>
      <Text>{t("certificates_description")}</Text>
      <Box mt={20}>
        <form onSubmit={form.onSubmit(handleSubmit)}>
          <SimpleGrid cols={{ cmd: 1, lg: 2 }} spacing={{ cmd: "sm" }}>
            <Box style={{ width: "300px", margin: "auto" }}>
              {getCertificateDetails.isSuccess &&
              getCertificateDetails.data?.data?.sampleUrl ? (
                <div style={{ position: "relative", backgroundColor: "black" }}>
                  <Image
                    radius={"md"}
                    style={{
                      opacity: "0.4",
                    }}
                    src={getCertificateDetails.data?.data?.sampleUrl}
                  />
                  <div
                    style={{
                      position: "absolute",
                      left: -20,
                      bottom: 0,
                      right: 0,
                      top: 0,
                      margin: "auto",
                      width: "45px",
                      height: "30px",
                      display: "flex",
                    }}
                  >
                    <Tooltip label={t("view_certificate")}>
                      <ActionIcon
                        variant="subtle"
                        onClick={() => window.open(data?.sampleUrl)}
                        mr={10}
                        size="md"
                      >
                        <IconEye />
                      </ActionIcon>
                    </Tooltip>
                    <Tooltip label={t("download_certificate")}>
                      <ActionIcon
                        variant="subtle"
                        onClick={() => {
                          downloadImage(
                            data?.sampleUrl ?? "",
                            data?.title ?? ""
                          );
                        }}
                      >
                        <IconDownload />
                      </ActionIcon>
                    </Tooltip>
                  </div>
                </div>
              ) : (
                <Box>{t("certificate_preview_details")}</Box>
              )}
              {getCertificateDetails.isSuccess &&
                getCertificateDetails.data?.data?.sampleUrl && (
                  <Text size={"xs"} c="dimmed">
                    {t("certificate_note")}
                  </Text>
                )}
            </Box>

            <Container fluid w={"100%"}>
              <Flex>
                <CustomTextFieldWithAutoFocus
                  w={"100%"}
                  label={t("title")}
                  withAsterisk
                  placeholder={t("course_certificate_title") as string}
                  {...form.getInputProps("title")}
                />
              </Flex>
              <Grid mt={20}>
                <Grid.Col span={6}>
                  <DatePickerInput
                    w={"100%"}
                    valueFormat="MMM DD, YYYY"
                    placeholder={t("start_date_placeholder") as string}
                    label={t("start_date")}
                    withAsterisk
                    leftSection={<IconCalendar size={16} />}
                    {...form.getInputProps("eventStartDate")}
                  />
                </Grid.Col>

                <Grid.Col span={6}>
                  <DatePickerInput
                    w={"100%"}
                    valueFormat="MMM DD, YYYY"
                    placeholder={t("end_date_placeholder") as string}
                    label={t("end_date")}
                    withAsterisk
                    minDate={form.values.eventStartDate}
                    leftSection={<IconCalendar size={16} />}
                    {...form.getInputProps("eventEndDate")}
                  />
                </Grid.Col>
              </Grid>
              <Button mt={20} type="submit" loading={addCertificate.isLoading}>
                {t("submit")}
              </Button>
            </Container>
          </SimpleGrid>
        </form>
        <div style={{ marginTop: "30px" }}>
          <Flex justify={"space-between"} mb={10}>
            <Text size={"xl"} fw="bold">
              {t("add_signatures")}
            </Text>
          </Flex>
          {getSignature.data?.map((cert) => (
            <CreateSignature
              data={cert}
              key={cert.id}
              onClose={() => {
                getCertificateDetails.refetch();
                setAddSignatureForm();
              }}
            />
          ))}
        </div>

        {addSignatureForm ? (
          <CreateSignature
            onClose={() => {
              getCertificateDetails.refetch();
              setAddSignatureForm();
            }}
          />
        ) : (
          getSignature.data &&
          getSignature.data?.length < 3 && (
            <Button onClick={() => setAddSignatureForm()}>
              {t("add_more")}
            </Button>
          )
        )}
      </Box>
    </>
  );
};

export default Certificate;
