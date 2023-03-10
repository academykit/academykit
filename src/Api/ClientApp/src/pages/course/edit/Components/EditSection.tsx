import InlineInput from "@components/Ui/InlineInput";
import {
  Box,
  Button,
  Container,
  createStyles,
  Grid,
  Text,
  Title,
} from "@mantine/core";
import CourseSection from "./Section";
import { useSection } from "@context/SectionProvider";
import {
  ISection,
  useCourseDescription,
  useCreateSection,
} from "@utils/services/courseService";
import { useParams } from "react-router-dom";
import { showNotification } from "@mantine/notifications";
import { useForm } from "@mantine/form";
import errorType from "@utils/services/axiosError";
import { IconDragDrop } from "@tabler/icons";
import { CourseStatus } from "@utils/enums";

const useStyle = createStyles((theme) => ({
  section: {
    background: theme.colorScheme === "dark" ? theme.black[2] : theme.white[2],
  },
}));

const EditSection = () => {
  const { classes } = useStyle();
  const section = useSection();

  const { id: slug } = useParams();

  const getCourseDetails: any = useCourseDescription(slug as string);

  return (
    <Container fluid>
      <Grid mt={20}>
        <Grid.Col span={section?.matches ? 10 : 12}>
          <Title mb={10}>Sections and Lessons</Title>
          <Text>
            You can add lessons and group them into sections. Add section, then
            add lessons within the section. You can arrange the order of the
            sections and lessons by dragging & dropping them by using the
            "Sorting Icon" {<IconDragDrop />}
          </Text>
        </Grid.Col>
      </Grid>

      <Box className={classes.section}>
        <Box mt={20}>
          {getCourseDetails.data && (
            <CourseSection
              data={getCourseDetails.data?.sections}
              status={getCourseDetails.data?.status}
              slug={slug as string}
            />
          )}
        </Box>
      </Box>

      {getCourseDetails.data?.status !== CourseStatus.Completed && (
        <div style={{ marginTop: "20px" }}>
          {!section?.isAddSection ? (
            <Button
              onClick={() => {
                section?.setIsAddSection(!section?.isAddSection);
                section?.setAddLessonClick(false);
              }}
            >
              Add New Section
            </Button>
          ) : (
            <AddSectionForm slug={slug as string} />
          )}
        </div>
      )}
    </Container>
  );
};

const AddSectionForm = ({ slug }: { slug: string }) => {
  const section = useSection();
  const form = useForm({
    initialValues: {
      name: "",
    },
  });
  const addSection = useCreateSection(slug);

  return (
    <form
      onSubmit={form.onSubmit(async (values) => {
        if (!addSection.isLoading) {
          try {
            const data = await addSection.mutateAsync({
              name: values.name,
              courseIdentity: slug,
            });
            section?.setActiveSection(data.data.slug);
            section?.setIsAddSection(!section?.isAddSection);
            showNotification({
              message: "Section Added successfully!",
            });
          } catch (error) {
            const err = errorType(error);
            showNotification({
              message: err,
              color: "red",
              title: "Error!",
            });
          }
          form.reset();
        }
      })}
    >
      <InlineInput
        placeholder="Enter section name"
        onCloseEdit={() => section?.setIsAddSection(!section?.isAddSection)}
        {...form.getInputProps("name")}
      ></InlineInput>
    </form>
  );
};

export default EditSection;
