import InlineInput from "@components/Ui/InlineInput";
import { useSection } from "@context/SectionProvider";
import useFormErrorHooks from "@hooks/useFormErrorHooks";
import {
  Box,
  Button,
  Container,
  Grid,
  Text,
  Title,
  useMantineColorScheme,
  useMantineTheme,
} from "@mantine/core";
import { useForm, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import { IconDragDrop } from "@tabler/icons-react";
import { CourseStatus } from "@utils/enums";
import errorType from "@utils/services/axiosError";
import {
  useCourseDescription,
  useCreateSection,
} from "@utils/services/courseService";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";
import * as Yup from "yup";
import CourseSection from "./Section";

const EditSection = () => {
  const section = useSection();
  const { colorScheme } = useMantineColorScheme();
  const theme = useMantineTheme();

  const { id: slug } = useParams();

  const getCourseDetails: any = useCourseDescription(slug as string);
  const { t } = useTranslation();
  return (
    <Container fluid>
      <Grid mt={20}>
        <Grid.Col span={section?.matches ? 10 : 12}>
          <Title mb={10}>{t("sections_and_lessons")}</Title>
          <Text>
            {t("group_lessons_sections")} {<IconDragDrop />}
          </Text>
        </Grid.Col>
      </Grid>

      <Box
        style={{
          background: colorScheme === "dark" ? theme.black[2] : theme.white[2],
        }}
      >
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
              {t("add_new_section")}
            </Button>
          ) : (
            <AddSectionForm slug={slug as string} />
          )}
        </div>
      )}
    </Container>
  );
};

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    name: Yup.string().required(t("section_name_required") as string),
  });
};

const AddSectionForm = ({ slug }: { slug: string }) => {
  const section = useSection();
  const form = useForm({
    initialValues: {
      name: "",
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  const addSection = useCreateSection(slug);
  const { t } = useTranslation();

  return (
    <form
      onSubmit={form.onSubmit(async (values) => {
        if (!addSection.isPending) {
          try {
            const data = await addSection.mutateAsync({
              name: values.name,
              courseIdentity: slug,
            });
            section?.setActiveSection(data.data.slug);
            section?.setIsAddSection(!section?.isAddSection);
            showNotification({
              message: t("section_add_success") as string,
            });
          } catch (error) {
            const err = errorType(error);
            showNotification({
              message: err,
              color: "red",
              title: t("error"),
            });
          }
          form.reset();
        }
      })}
    >
      <InlineInput
        placeholder={t("section_name_placeholder")}
        onCloseEdit={() => section?.setIsAddSection(!section?.isAddSection)}
        {...form.getInputProps("name")}
      ></InlineInput>
    </form>
  );
};

export default EditSection;
