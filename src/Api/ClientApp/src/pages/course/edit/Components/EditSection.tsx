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
  useGetSection,
} from "@utils/services/courseService";
import { useLocation, useParams } from "react-router-dom";
import { showNotification } from "@mantine/notifications";
import { UseQueryResult } from "@tanstack/react-query";
import { IPaginated } from "@utils/services/types";
import { useForm } from "@mantine/form";
import errorType from "@utils/services/axiosError";

const useStyle = createStyles((theme) => ({
  section: {
    background: theme.colorScheme === "dark" ? theme.black[2] : theme.white[2],
  },
}));

const EditSection = () => {
  const { classes } = useStyle();
  const section = useSection();

  const { id: slug } = useParams();

  const getSection: UseQueryResult<IPaginated<ISection>> = useGetSection(
    slug as string
  );
  const getCourseDetails: any = useCourseDescription(slug as string);

  return (
    <Container fluid>
      <Grid mt={20}>
        <Grid.Col span={section?.matches ? 10 : 12}>
          <Title>Sections and Lessons</Title>
          <Text>
            You can add lessons and group them into sections. Add section, then
            add lessons within the section.
          </Text>
        </Grid.Col>
      </Grid>
      <Box className={classes.section}>
        <Box mt={20}>
          {getSection.data && (
            <CourseSection
              data={getCourseDetails.data?.sections}
              slug={slug as string}
            />
          )}
        </Box>
      </Box>
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
