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
  TextInput,
  Title,
  Tooltip,
} from "@mantine/core";
import { DatePicker } from "@mantine/dates";
import { useToggle } from "@mantine/hooks";
import { IconCalendar, IconEye, IconDownload } from "@tabler/icons";
import {
  useAddCertificate,
  useGetCertificateDetails,
  useGetSignature,
} from "@utils/services/courseService";
import { useForm, yupResolver } from "@mantine/form";
import { useParams } from "react-router-dom";
import CreateSignature from "./Components/Signature/CreateSignature";
import { showNotification } from "@mantine/notifications";
import * as Yup from "yup";
import errorType from "@utils/services/axiosError";
import { useEffect } from "react";
import moment from "moment";
import downloadImage from "@utils/downloadImage";

const schema = Yup.object().shape({
  title: Yup.string().required("Course Title is required."),
  eventStartDate: Yup.string()
    .required("Event Start Date is required.")
    .typeError("Event Start Date is required."),
  eventEndDate: Yup.string()
    .required("Event End Date is required.")
    .typeError("Event End Date is required."),
});

const Certificate = () => {
  const params = useParams();
  const getSignature = useGetSignature(params.id as string);
  const addCertificate = useAddCertificate(params.id as string);
  const getCertificateDetails = useGetCertificateDetails(params.id as string);
  const data = getCertificateDetails.data?.data;

  const form = useForm({
    validate: yupResolver(schema),
    initialValues: {
      title: "",
      eventStartDate: new Date(),
      eventEndDate: new Date(),
    },
  });

  useEffect(() => {
    if (
      getCertificateDetails.isSuccess &&
      getCertificateDetails.data?.status === 200
    ) {
      const data = getCertificateDetails.data;

      form.setValues({
        title: data.data.title,
        eventEndDate: moment(data.data.eventEndDate + "z")
          .local()
          .toDate(),
        eventStartDate: moment(data.data.eventStartDate + "z")
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
        title: "Success",
        message: `Certificate Details ${
          isEditing ? "Updated" : "Added"
        } successfully!`,
      });
    } catch (error) {
      const err = errorType(error);
      showNotification({
        title: "Error",
        message: err,
        color: "red",
      });
    }
  };

  return (
    <>
      <Title>Certificates</Title>
      <Text>
        You can add the contents and signature of essential persons regarding
        the certificates here by viewing the default template.
      </Text>
      <Box mt={20}>
        <form onSubmit={form.onSubmit(handleSubmit)}>
          <SimpleGrid
            cols={2}
            breakpoints={[{ maxWidth: 1050, cols: 1, spacing: "sm" }]}
          >
            <Box sx={{ width: "300px", margin: "auto" }}>
              {getCertificateDetails.isSuccess &&
                getCertificateDetails.data?.data?.sampleUrl && (
                  <div
                    style={{ position: "relative", backgroundColor: "black" }}
                  >
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
                      <Tooltip label="View Certificate">
                        <ActionIcon
                          variant="default"
                          onClick={() => window.open(data?.sampleUrl)}
                          mr={10}
                          size="md"
                        >
                          <IconEye />
                        </ActionIcon>
                      </Tooltip>
                      <Tooltip label="Download Certificate">
                        <ActionIcon
                          variant="default"
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
                )}
              {getCertificateDetails.isSuccess &&
                getCertificateDetails.data?.data?.sampleUrl && (
                  <Text size={"xs"} c="dimmed">
                    Note: You need atleast one signature to be able to issue to
                    trainee.
                  </Text>
                )}
            </Box>

            <Container fluid w={"100%"}>
              <Flex>
                <TextInput
                  w={"100%"}
                  label="Title"
                  withAsterisk
                  placeholder="Course Title for certificate"
                  {...form.getInputProps("title")}
                />
              </Flex>
              <Grid mt={20}>
                <Grid.Col span={6}>
                  <DatePicker
                    w={"100%"}
                    placeholder="Pick Starting Date"
                    label="Start date"
                    withAsterisk
                    icon={<IconCalendar size={16} />}
                    {...form.getInputProps("eventStartDate")}
                  />
                </Grid.Col>

                <Grid.Col span={6}>
                  <DatePicker
                    w={"100%"}
                    placeholder="Pick Ending Date"
                    label="End date"
                    withAsterisk
                    minDate={form.values.eventStartDate}
                    icon={<IconCalendar size={16} />}
                    {...form.getInputProps("eventEndDate")}
                  />
                </Grid.Col>
              </Grid>
              <Button mt={20} type="submit" loading={addCertificate.isLoading}>
                Submit
              </Button>
            </Container>
          </SimpleGrid>
        </form>
        <div style={{ marginTop: "30px" }}>
          <Flex justify={"space-between"} mb={10}>
            <Text size={"xl"} weight="bold">
              Add Signatures
            </Text>
          </Flex>
          {getSignature.data?.map((cert) => (
            <CreateSignature
              data={cert}
              key={cert.id}
              onClose={() => setAddSignatureForm()}
            />
          ))}
        </div>

        {addSignatureForm ? (
          <CreateSignature onClose={() => setAddSignatureForm()} />
        ) : (
          getSignature.data &&
          getSignature.data?.length < 3 && (
            <Button onClick={() => setAddSignatureForm()}>Add More</Button>
          )
        )}
      </Box>
    </>
  );
};

export default Certificate;
