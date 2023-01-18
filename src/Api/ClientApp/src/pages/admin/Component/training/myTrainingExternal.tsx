import ThumbnailEditor from "@components/Ui/ThumbnailEditor";
import useAuth from "@hooks/useAuth";
import {
  ActionIcon,
  Box,
  Button,
  Card,
  Flex,
  Group,
  Badge,
  Image,
  Modal,
  Text,
  TextInput,
} from "@mantine/core";
import { DateRangePicker } from "@mantine/dates";
import { createFormContext, yupResolver } from "@mantine/form";
import { useToggle } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconDownload, IconEdit, IconEye } from "@tabler/icons";
import downloadImage from "@utils/downloadImage";
import { UserRole } from "@utils/enums";
import errorType from "@utils/services/axiosError";
import {
  CertificateStatus,
  useAddCertificate,
  useGetExternalCertificate,
  useGetUserCertificate,
  useUpdateCertificate,
  useUpdateCertificateStatus,
} from "@utils/services/certificateService";
import moment from "moment";
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import * as Yup from "yup";

const [FormProvider, useFormContext, useForm] = createFormContext();

const schema = Yup.object().shape({
  name: Yup.string().required("Certificate name is required."),
  duration: Yup.number().typeError("Duration must be in hours."),
});

// const EditModal = ({values}:{values: any}) => {
//   const form = useForm({
//     initialValues: {
//       name: values?.name,
//       duration: values?.duration,
//       location: values?.location,
//       institute: values?.institute,
//       imageUrl: values?.imageUrl,
//     },
//     validate: yupResolver(schema),
//   });

//   const [openId, setOpenId] = useState('')

//   const update = useUpdateCertificate(values?.id)

//   return

// };

const MyTrainingExternal = ({ isAdmin }: { isAdmin?: boolean }) => {
  const [showConfirmation, setShowConfirmation] = useToggle();
  const { id } = useParams();
  const [value, setValue] = useState<[Date, Date]>([new Date(), new Date()]);
  const addCertificate = useAddCertificate();
  const certificateList = useGetExternalCertificate(id ? false : true);
  const userCertificate = useGetUserCertificate(id as string);
  const update = useUpdateCertificate(id as string);
  const [idd, setIdd] = useState<any>();
  const [updates, setUpdates] = useState(false);

  const form = useForm({
    initialValues: {
      name: "",
      duration: 0,
      location: "",
      institute: "",
      imageUrl: "",
    },
    validate: yupResolver(schema),
  });

  useEffect(() => {
    if (idd) {
      setShowConfirmation();
      form.setValues({
        name: idd?.name,
        duration: idd?.duration,
        location: idd?.location,
        institute: idd?.institute,
        imageUrl: idd?.imageUrl,
      });
    }
  }, [idd]);

  const handleSubmit = async (data: any) => {
    data = { ...data, startDate: value[0], endDate: value[1] };
    try {
      if (updates) {
        await update.mutateAsync({ data, id: idd?.id });
      } else {
        await addCertificate.mutateAsync(data);
      }
      showNotification({
        message: "Certificate added successfully.",
      });
      form.reset();
    } catch (error) {
      const err = errorType(error);
      showNotification({
        color: "red",
        message: err,
      });
    }
    setShowConfirmation();
    setIdd(() => null);
    setUpdates(() => false);
  };

  const auth = useAuth();
  return (
    <div>
      <Modal
        title="Add new Certificate"
        opened={showConfirmation}
        onClose={() => {
          setShowConfirmation();
          setIdd(null);
          setUpdates(false);
        }}
        styles={{
          title: {
            fontWeight: "bold",
          },
        }}
      >
        <FormProvider form={form}>
          <form onSubmit={form.onSubmit(handleSubmit)}>
            <TextInput
              label="Name"
              name="name"
              withAsterisk
              {...form.getInputProps("name")}
            />
            <TextInput
              label="Duration"
              name="duration"
              {...form.getInputProps("duration")}
            />
            <DateRangePicker
              label="Start Date - End Date"
              placeholder="Pick dates range"
              value={value}
              //@ts-ignore
              onChange={setValue}
            />
            <TextInput
              label="Location"
              name="location"
              {...form.getInputProps("location")}
            />
            <TextInput
              label="Institute"
              name="institute"
              {...form.getInputProps("institute")}
            />
            <Text>Certificate Image</Text>
            <ThumbnailEditor
              formContext={useFormContext}
              label="Certificate Image"
              FormField="imageUrl"
              currentThumbnail={idd?.imageUrl}
            />
            <Button type="submit" loading={addCertificate.isLoading}>
              Submit
            </Button>
          </form>
        </FormProvider>
      </Modal>
      <Group position="right" onClick={() => setShowConfirmation()}>
        <Button>Add Certificate</Button>
      </Group>
      {certificateList.isSuccess && certificateList.data?.length < 0 && (
        <Box>No External trainings found.</Box>
      )}
      {certificateList.isSuccess &&
        certificateList.data.map((x) => (
          <Card withBorder mt={10}>
            <Flex justify={"space-between"}>
              <Box>
                <Text weight={"bold"}>
                  {x.name}
                  <Badge ml={20}>{CertificateStatus[x.status]}</Badge>
                  <ActionIcon
                    onClick={() => {
                      setIdd(x);
                      setUpdates(true);
                    }}
                  >
                    <IconEdit />
                  </ActionIcon>
                </Text>
                <Text mt={5}>
                  From {moment(x.startDate).format("YYYY-MM-DD")} to{" "}
                  {moment(x.endDate).format("YYYY-MM-DD")} completed in about{" "}
                  {x.duration} hrs.
                </Text>
                <Text>
                  {x.institute}, {x.location}
                </Text>
              </Box>
              <Box
                style={{ width: 150, marginTop: "auto", marginBottom: "auto" }}
              >
                {x.imageUrl && (
                  <div style={{ position: "relative" }}>
                    <Image
                      src={x.imageUrl || ""}
                      radius="md"
                      style={{
                        opacity: "0.5",
                      }}
                    />
                    <div
                      style={{
                        position: "absolute",
                        left: 0,
                        bottom: 0,
                        right: 0,
                        margin: "auto",
                        top: 0,
                        width: "45px",
                        height: "30px",
                        display: "flex",
                      }}
                    >
                      <ActionIcon
                        onClick={() => window.open(x.imageUrl)}
                        mr={10}
                      >
                        <IconEye color="black" />
                      </ActionIcon>
                      <ActionIcon
                        onClick={() =>
                          downloadImage(x.imageUrl, x.user.fullName ?? "")
                        }
                      >
                        <IconDownload color="black" />
                      </ActionIcon>
                    </div>
                  </div>
                )}
              </Box>
            </Flex>
            {auth?.auth &&
              auth.auth.role <= UserRole.Admin &&
              auth.auth.id !== x.user.id && (
                <Box mt={10}>
                  <Button>Approve</Button>
                  <Button ml={10} variant="outline" color={"red"}>
                    Reject
                  </Button>
                </Box>
              )}
          </Card>
        ))}
    </div>
  );
};

export default MyTrainingExternal;
