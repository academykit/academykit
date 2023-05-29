import TextEditor from "@components/Ui/TextEditor";
import ThumbnailEditor from "@components/Ui/ThumbnailEditor";
import {
  Box,
  Button,
  createStyles,
  Group,
  Loader,
  MultiSelect,
  Select,
  Text,
  TextInput,
} from "@mantine/core";
import { createFormContext, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import queryStringGenerator from "@utils/queryStringGenerator";
import RoutePath from "@utils/routeConstants";
import errorType from "@utils/services/axiosError";
import {
  useCourseDescription,
  useUpdateCourse,
} from "@utils/services/courseService";
import { useGroups } from "@utils/services/groupService";
import { useLevels } from "@utils/services/levelService";
import { useAddTag, useTags } from "@utils/services/tagService";
import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import * as Yup from "yup";

const useStyle = createStyles((theme, _params, getRef) => ({
  group: {
    [theme.fn.smallerThan(theme.breakpoints.md)]: {
      flexDirection: "column",
      alignItems: "start",
      justifyContent: "stretch",

      "& > div": {
        margin: 0,
        width: "100%",
        maxWidth: "500px",
      },
    },
  },
}));

interface FormValues {
  thumbnail: string;
  title: string;
  level: any;
  groups: string;
  description: string;
  tags: string[];
}

const schema = Yup.object().shape({
  title: Yup.string().required("Course Title is required."),
  level: Yup.string().required("Level is required."),
  groups: Yup.string().required("Group is required."),
});

export const [FormProvider, useFormContext, useForm] =
  createFormContext<FormValues>();
const EditCourse = () => {
  const { classes } = useStyle();
  const form = useForm({
    initialValues: {
      thumbnail: "",
      title: "",
      level: "",
      groups: "",
      description: "",
      tags: [],
    },
    validate: yupResolver(schema),
  });

  const [searchParams, setSearchParams] = useState("");
  const [searchParamsGroup, setSearchParamsGroup] = useState("");

  const label = useLevels();
  const { mutate, data: addTagData, isSuccess } = useAddTag();

  const tags = useTags(
    queryStringGenerator({
      search: searchParams,
      size: 10000,
    })
  );

  const groups = useGroups(
    queryStringGenerator({
      search: searchParamsGroup,
      size: 10000,
    })
  );

  const slug = useParams();
  const {
    data: courseSingleData,
    isLoading,
    isSuccess: courseIsSuccess,
  } = useCourseDescription(slug.id as string);

  const [tagsList, setTagsList] = useState<{ value: string; label: string }[]>(
    []
  );

  useEffect(() => {
    if (label.isSuccess) {
      form.setFieldValue("level", courseSingleData?.levelId);
    }
  }, [label.isSuccess, courseIsSuccess]);

  useEffect(() => {
    if (groups.isSuccess) {
      form.setFieldValue("groups", courseSingleData?.groupId as string);
    }
  }, [groups.isSuccess, courseIsSuccess]);

  useEffect(() => {
    if (tags.isSuccess) {
      setTagsList(tags.data.items.map((x) => ({ label: x.name, value: x.id })));
      courseSingleData?.tags &&
        form.setFieldValue("tags", [
          ...form.values.tags,
          ...courseSingleData?.tags.map((x) => x.tagId),
        ]);
    }
  }, [tags.isSuccess, courseIsSuccess]);

  useEffect(() => {
    if (isSuccess) {
      setTagsList([
        ...tagsList,
        { label: addTagData.data.name, value: addTagData.data.id },
      ]);
    }
  }, [isSuccess]);

  useEffect(() => {
    if (courseIsSuccess) {
      form.setValues({
        thumbnail: courseSingleData.thumbnailUrl,
        title: courseSingleData.name,
        description: courseSingleData.description,
      });
    }
  }, [courseIsSuccess]);

  const updateCourse = useUpdateCourse(slug.id as string);
  const navigator = useNavigate();

  const submitHandler = async (data: FormValues) => {
    try {
      const res = await updateCourse.mutateAsync({
        description: data.description,
        groupId: data.groups,
        tagIds: data.tags,
        levelId: data.level,
        language: 1,
        name: data.title.trim().split(/ +/).join(" "),
        thumbnailUrl: data.thumbnail,
      });
      navigator(RoutePath.manageCourse.lessons(slug.id).route);
      showNotification({
        title: "Success",
        message: "Training updated successfully.",
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
    <div>
      <FormProvider form={form}>
        <form onSubmit={form.onSubmit(submitHandler)}>
          <Box mt={20}>
            <ThumbnailEditor
              formContext={useFormContext}
              currentThumbnail={courseSingleData?.thumbnailUrl}
              label="thumbnail"
            />
            <Group mt={10} grow>
              <TextInput
                placeholder="Course Title awsdfas"
                label="Title"
                withAsterisk
                {...form.getInputProps("title")}
                size="lg"
              />
            </Group>

            <Group grow mt={20}>
              {tags.isSuccess ? (
                <MultiSelect
                  searchable
                  labelProps="name"
                  creatable
                  sx={{ maxWidth: "500px" }}
                  data={tagsList}
                  {...form.getInputProps("tags")}
                  getCreateLabel={(query) => `+ Create ${query}`}
                  onCreate={(query) => {
                    mutate(query);
                  }}
                  size={"lg"}
                  label="Tags"
                  placeholder="Please select Tags."
                />
              ) : (
                <Loader />
              )}
              {label.isSuccess ? (
                <Select
                  withAsterisk
                  size="lg"
                  label="Level"
                  placeholder="Please select Level."
                  {...form.getInputProps("level")}
                  data={label.data.map((x) => ({ value: x.id, label: x.name }))}
                ></Select>
              ) : (
                <Loader />
              )}
            </Group>
            {!groups.isLoading ? (
              <Select
                mt={20}
                searchable
                withAsterisk
                labelProps="name"
                sx={{ maxWidth: "500px" }}
                data={
                  groups.data &&
                  groups.data.data.items.map((x) => ({
                    label: x.name,
                    value: x.id,
                  }))
                }
                {...form.getInputProps("groups")}
                size={"lg"}
                label="Group"
                placeholder="Please select Group."
              />
            ) : (
              <Loader />
            )}
            <Box mt={20}>
              <Text>Description</Text>
              <TextEditor formContext={useFormContext} />
            </Box>
            <Box mt={20}>
              <Button size="lg" type="submit" loading={updateCourse.isLoading}>
                Submit
              </Button>
            </Box>
          </Box>
        </form>
      </FormProvider>
    </div>
  );
};

export default EditCourse;
