import {
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
} from "@mantine/core";
import { DatePicker } from "@mantine/dates";
import { useToggle } from "@mantine/hooks";
import { IconCalendar } from "@tabler/icons";
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
              {/* <AspectRatio ratio={16 / 9}> */}
              <Image src={getCertificateDetails.data?.data?.sampleUrl} />
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
