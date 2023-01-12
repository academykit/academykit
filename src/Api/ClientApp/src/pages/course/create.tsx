import Breadcrumb from "@components/Ui/BreadCrumb";
import TextEditor from "@components/Ui/TextEditor";
import ThumbnailEditor from "@components/Ui/ThumbnailEditor";
import {
  Box,
  Button,
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
import { useCreateCourse } from "@utils/services/courseService";
import { useGroups } from "@utils/services/groupService";
import { useLevels } from "@utils/services/levelService";
import { useAddTag, useTags } from "@utils/services/tagService";
import { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import * as Yup from "yup";

interface FormValues {
  thumbnail: string;
  title: string;
  level: any;
  groups: string;
  description: string;
  tags: string[];
}
const schema = Yup.object().shape({
  title: Yup.string().trim().required("Course Title is required."),
  level: Yup.string().required("Level is required."),
  groups: Yup.string().nullable().required("Group is required."),
});

export const [FormProvider, useFormContext, useForm] =
  createFormContext<FormValues>();

const CreateCoursePage = () => {
  const [searchParamGroup, setsearchParamGroup] = useState("");

  const groups = useGroups(
    queryStringGenerator({
      search: searchParamGroup,
      size: 10000,
    })
  );
  const [searchParams] = useSearchParams();
  const groupSlug = searchParams.get("group");
  useEffect(() => {
    if (groups.isSuccess && groups?.data && groupSlug) {
      form.setFieldValue(
        "groups",
        (
          groups.data &&
          groups.data.data.items.find((x) => x.slug === groupSlug)
        )?.id ?? ""
      );
    }
  }, [groups.isSuccess]);

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

  const [searchParam, setsearchParam] = useState("");

  const label = useLevels();
  const { mutate, data: addTagData, isSuccess } = useAddTag();
  const navigate = useNavigate();

  const tags = useTags(
    queryStringGenerator({
      search: searchParam,
      size: 10000,
    })
  );

  const [tagsList, setTagsList] = useState<{ value: string; label: string }[]>(
    []
  );
  useEffect(() => {
    if (tags.isSuccess && tags.isFetched) {
      setTagsList(tags.data.items.map((x) => ({ label: x.name, value: x.id })));
    }
  }, [tags.isSuccess]);

  useEffect(() => {
    if (isSuccess) {
      setTagsList([
        ...tagsList,
        { label: addTagData.data.name, value: addTagData.data.id },
      ]);
      form.setFieldValue("tags", [...form.values.tags, addTagData?.data?.id]);
    }
  }, [isSuccess]);

  const { mutateAsync, isLoading } = useCreateCourse();

  const submitHandler = async (data: FormValues) => {
    try {
      const res = await mutateAsync({
        description: data.description,
        groupId: data.groups,
        tagIds: data.tags,
        levelId: data.level,
        language: 1,
        name: data.title.trim().split(/ +/).join(" "),
        thumbnailUrl: data.thumbnail,
      });
      form.reset();
      showNotification({
        title: "Success!",
        message: "Course Created successfully!",
      });
      navigate(RoutePath.manageCourse.lessons(res.data.slug).route);
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
      <Breadcrumb />
      <FormProvider form={form}>
        <form onSubmit={form.onSubmit(submitHandler)}>
          <Box mt={20}>
            <ThumbnailEditor formContext={useFormContext} />
            <Group mt={10} grow>
              <TextInput
                placeholder="Course Title."
                label="Title"
                name="Title"
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
                  value={[]}
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
                  placeholder="Please select Level."
                  label="Level"
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
              <Button size="lg" type="submit" loading={isLoading}>
                Submit
              </Button>
            </Box>
          </Box>
        </form>
      </FormProvider>
    </div>
  );
};

export default CreateCoursePage;
