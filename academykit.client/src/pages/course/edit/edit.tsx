import { DynamicAutoFocusTextField } from "@components/Ui/CustomTextFieldWithAutoFocus";
import GroupCreatableSelect from "@components/Ui/GroupCreatableSelect";
import RichTextEditor from "@components/Ui/RichTextEditor/Index";
import TextViewer from "@components/Ui/RichTextViewer";
import ThumbnailEditor from "@components/Ui/ThumbnailEditor";
import useAuth from "@hooks/useAuth";
import useCustomForm from "@hooks/useCustomForm";
import { useDropdownUtils } from "@hooks/useDropdownUtils";
import useFormErrorHooks from "@hooks/useFormErrorHooks";
import {
  Accordion,
  ActionIcon,
  Box,
  Button,
  Checkbox,
  Flex,
  Group,
  Loader,
  Select,
  Text,
} from "@mantine/core";
import { DatePickerInput } from "@mantine/dates";
import { createFormContext, yupResolver } from "@mantine/form";
import { useScrollIntoView } from "@mantine/hooks";
import { showNotification } from "@mantine/notifications";
import { IconPlus, IconTrash } from "@tabler/icons-react";
import { TrainingEligibilityEnum } from "@utils/enums";
import queryStringGenerator from "@utils/queryStringGenerator";
import RoutePath from "@utils/routeConstants";
import errorType from "@utils/services/axiosError";
import {
  useCourseDescription,
  useUpdateCourse,
} from "@utils/services/courseService";
import { useAddGroup, useGroups } from "@utils/services/groupService";
import { useLevels } from "@utils/services/levelService";
import { ITag, useAddTag, useTags } from "@utils/services/tagService";
import moment from "moment";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useNavigate, useParams } from "react-router-dom";

import TagMultiSelectCreatable from "../component/TagMultiSelectCreatable";
import { schema } from "../create";

interface FormValues {
  thumbnail: string;
  title: string;
  level: any;
  groups: string;
  description: string;
  tags: string[];
  language: string;
  startDate: Date | null;
  endDate: Date | null;
  isUnlimitedEndDate: boolean;
  trainingEligibilities: {
    eligibility: string;
    eligibilityId: string;
  }[];
}

export const [FormProvider, useFormContext, useForm] =
  createFormContext<FormValues>();
const EditCourse = () => {
  const slug = useParams();
  const [viewMode, setViewMode] = useState(true);
  const cForm = useCustomForm();
  const { t } = useTranslation();

  const {
    getDepartmentDropdown,
    getTrainingDropdown,
    getSkillDropdown,
    getAssessmentDropdown,
  } = useDropdownUtils();

  const { scrollIntoView: scrollToTop, targetRef: refBasic } =
    useScrollIntoView<HTMLDivElement>({
      offset: 60,
    });

  const form = useForm({
    initialValues: {
      thumbnail: "",
      title: "",
      level: "",
      groups: "",
      description: "",
      tags: [],
      language: "1",
      startDate: null,
      endDate: null,
      isUnlimitedEndDate: false,
      trainingEligibilities: [],
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  const [searchParams] = useState("");
  const [searchParamsGroup] = useState("");
  const groupAdd = useAddGroup();
  const auth = useAuth();

  const label = useLevels();
  const { data: addTagData, isSuccess, mutateAsync: mutAsync } = useAddTag();
  const [language] = useState([{ value: "1", label: "English" }]);

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

  const {
    data: courseSingleData,
    isSuccess: courseIsSuccess,
    refetch,
  } = useCourseDescription(slug.id as string);

  const getEligibilityType = () => {
    return Object.entries(TrainingEligibilityEnum)
      .splice(0, Object.entries(TrainingEligibilityEnum).length / 2)
      .map(([key, value]) => ({
        value: key,
        label: t(value.toString()),
      }));
  };

  const [tagsLists, setTagsLists] = useState<ITag[]>([]);

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
      // setTagsList(tags.data.items.map((x) => ({ label: x.name, value: x.id })));
      setTagsLists(tags.data.items.map((x) => x));
      courseSingleData?.tags &&
        form.setFieldValue("tags", [
          ...(form.values.tags ?? []),
          ...(courseSingleData?.tags?.map((x) => x.tagId) ?? []),
        ]);
    }
  }, [tags.isSuccess, courseIsSuccess]);

  useEffect(() => {
    if (isSuccess) {
      // setTagsList([
      //   ...tagsList,
      //   { label: addTagData.data.name, value: addTagData.data.id },
      // ]);
      setTagsLists([...tagsLists, addTagData.data]);
    }
  }, [isSuccess]);

  useEffect(() => {
    if (courseIsSuccess) {
      const eligibility =
        courseSingleData.trainingEligibilities &&
        courseSingleData.trainingEligibilities.map((x) => {
          return {
            eligibility: x.eligibility.toString(),
            eligibilityId: x.eligibilityId,
          };
        });
      form.setValues({
        thumbnail: courseSingleData.thumbnailUrl,
        title: courseSingleData.name,
        description: courseSingleData.description,
        language: courseSingleData.language.toString(),
        isUnlimitedEndDate: courseSingleData.isUnlimitedEndDate,
        startDate: moment(courseSingleData.startDate).toDate(),
        endDate: courseSingleData.endDate
          ? moment(courseSingleData.endDate).toDate()
          : null,
        trainingEligibilities: eligibility ?? [],
      });
    }
  }, [courseIsSuccess]);

  const updateCourse = useUpdateCourse(slug.id as string);
  const navigator = useNavigate();

  const submitHandler = async (values: typeof form.values) => {
    try {
      await updateCourse.mutateAsync({
        name: values.title,
        thumbnailUrl: values.thumbnail,
        description: values.description,
        groupId: values.groups,
        language: parseInt(values.language),
        duration: 0,
        levelId: values.level,
        tagIds: values.tags,
        startDate: moment(values.startDate)
          .add(5, "hour")
          .add(45, "minute")
          .toISOString(),
        endDate: moment(values.endDate)
          .add(5, "hour")
          .add(45, "minute")
          .toISOString(),
        isUnlimitedEndDate: values.isUnlimitedEndDate,
        trainingEligibilities: values.trainingEligibilities.map(
          (eligibility) => {
            return {
              eligibility: Number(eligibility.eligibility),
              eligibilityId: eligibility.eligibilityId,
            };
          }
        ),
      });
      refetch();
      navigator(RoutePath.manageCourse.lessons(slug.id).route);
      showNotification({
        title: t("success"),
        message: t("training_update_success"),
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };

  // set endDate as null if unlimited endDate is checked
  useEffect(() => {
    if (form.values.isUnlimitedEndDate) {
      form.setFieldValue("endDate", null);
    }
  }, [form.values.isUnlimitedEndDate]);

  // scroll to error section
  const handleError = (errors: typeof form.errors) => {
    if (
      errors.title ||
      errors.level ||
      errors.groups ||
      errors.startDate ||
      errors.endDate
    ) {
      scrollToTop({
        alignment: "center",
      });
    }
  };

  return (
    <div>
      <FormProvider form={form}>
        <form onSubmit={form.onSubmit(submitHandler, handleError)}>
          <Box mt={20}>
            <ThumbnailEditor
              formContext={useFormContext}
              currentThumbnail={courseSingleData?.thumbnailUrl}
              label={t("thumbnail") as string}
              disabled={viewMode}
            />
            <Group mt={10} grow ref={refBasic}>
              <DynamicAutoFocusTextField
                isViewMode={viewMode}
                readOnly={viewMode}
                placeholder={t("title_course") as string}
                label={t("title")}
                withAsterisk
                {...form.getInputProps("title")}
                size="lg"
              />
            </Group>

            <Group grow mt={20}>
              {tags.isSuccess ? (
                <TagMultiSelectCreatable
                  data={tagsLists ?? []}
                  mutateAsync={mutAsync}
                  form={form}
                  existingTags={courseSingleData}
                  size="lg"
                  readOnly={viewMode}
                />
              ) : (
                <div>
                  <Loader size={"xs"} />
                </div>
              )}
              {label.isSuccess ? (
                <Select
                  styles={{
                    input: {
                      border: viewMode ? "none" : "",
                      cursor: viewMode ? "text !important" : "",
                    },
                  }}
                  readOnly={viewMode}
                  withAsterisk
                  size="lg"
                  label={t("level")}
                  placeholder={t("level_placeholder") as string}
                  {...form.getInputProps("level")}
                  data={label.data.map((x) => ({ value: x.id, label: x.name }))}
                ></Select>
              ) : (
                <div>
                  <Loader size={"xs"} />
                </div>
              )}
            </Group>
            <Group grow mt={20}>
              {!groups.isLoading ? (
                <GroupCreatableSelect
                  api={groupAdd}
                  form={form}
                  data={groups?.data?.data?.items}
                  size="lg"
                  auth={auth}
                  readOnly={viewMode}
                />
              ) : (
                <Loader style={{ flexGrow: "0" }} />
              )}

              <Select
                readOnly={viewMode}
                styles={{
                  input: {
                    border: viewMode ? "none" : "",
                    cursor: viewMode ? "text !important" : "",
                  },
                }}
                label={t("Language")}
                size={"lg"}
                data={language}
                {...form.getInputProps("language")}
              />
            </Group>

            <Group grow mt={20}>
              <DatePickerInput
                readOnly={viewMode}
                withAsterisk
                label={t("start_date")}
                placeholder={t("pick_date") as string}
                size="lg"
                {...form.getInputProps("startDate")}
              />

              <DatePickerInput
                withAsterisk={!form.values.isUnlimitedEndDate}
                readOnly={viewMode}
                disabled={form.values.isUnlimitedEndDate}
                label={t("end_date")}
                placeholder={t("pick_date") as string}
                size="lg"
                {...form.getInputProps("endDate")}
              />
            </Group>
            <Group grow mt={20}>
              <Checkbox
                disabled={viewMode}
                label="Unlimited end date"
                {...form.getInputProps("isUnlimitedEndDate", {
                  type: "checkbox",
                })}
              />
            </Group>

            <Box mt={20}>
              <Text>{t("description")}</Text>
              {!viewMode && (
                <RichTextEditor
                  placeholder={t("course_description") as string}
                  formContext={useFormContext}
                />
              )}
              {viewMode && (
                <TextViewer content={courseSingleData?.description as string} />
              )}
            </Box>

            <Accordion defaultValue="Eligibility" mt={10}>
              <Accordion.Item value="Eligibility">
                <Accordion.Control>
                  {t("eligibility_criteria")}
                </Accordion.Control>
                <Accordion.Panel>
                  {form.values.trainingEligibilities.length < 1 && (
                    <Button
                      disabled={viewMode}
                      onClick={() => {
                        form.insertListItem(
                          "trainingEligibilities",
                          { eligibility: 0, eligibilityId: "" },
                          0
                        );
                      }}
                    >
                      {t("add_eligibility_criteria")}
                    </Button>
                  )}

                  {form.values.trainingEligibilities.map(
                    (_eligibility, index) => (
                      <Flex gap={10} key={index} align={"flex-end"} mb={10}>
                        <Select
                          disabled={viewMode}
                          allowDeselect={false}
                          label={t("eligibility_type")}
                          placeholder={t("pick_value") as string}
                          data={getEligibilityType() ?? []}
                          {...form.getInputProps(
                            `trainingEligibilities.${index}.eligibility`
                          )}
                        />
                        {form.values.trainingEligibilities[index].eligibility ==
                          TrainingEligibilityEnum.Department.toString() && (
                          <Select
                            withAsterisk
                            disabled={viewMode}
                            allowDeselect={false}
                            label={t("department")}
                            placeholder={t("pick_value") as string}
                            data={getDepartmentDropdown() ?? []}
                            {...form.getInputProps(
                              `trainingEligibilities.${index}.eligibilityId`
                            )}
                          />
                        )}

                        {form.values.trainingEligibilities[index].eligibility ==
                          TrainingEligibilityEnum.Training.toString() && (
                          <Select
                            disabled={viewMode}
                            withAsterisk
                            allowDeselect={false}
                            label={t("training")}
                            placeholder={t("pick_value") as string}
                            data={getTrainingDropdown() ?? []}
                            {...form.getInputProps(
                              `trainingEligibilities.${index}.eligibilityId`
                            )}
                          />
                        )}

                        {form.values.trainingEligibilities[index].eligibility ==
                          TrainingEligibilityEnum.Skills.toString() && (
                          <Select
                            disabled={viewMode}
                            withAsterisk
                            allowDeselect={false}
                            label={t("skills")}
                            placeholder={t("pick_value") as string}
                            data={getSkillDropdown() ?? []}
                            {...form.getInputProps(
                              `trainingEligibilities.${index}.eligibilityId`
                            )}
                          />
                        )}

                        {form.values.trainingEligibilities[index].eligibility ==
                          TrainingEligibilityEnum.Assessment.toString() && (
                          <Select
                            disabled={viewMode}
                            withAsterisk
                            allowDeselect={false}
                            label={t("assessment")}
                            placeholder={t("pick_value") as string}
                            data={getAssessmentDropdown() ?? []}
                            {...form.getInputProps(
                              `trainingEligibilities.${index}.eligibilityId`
                            )}
                          />
                        )}

                        {!viewMode && (
                          <ActionIcon
                            variant="subtle"
                            onClick={() => {
                              form.insertListItem(
                                "trainingEligibilities",
                                { eligibility: 0, eligibilityId: "" },
                                index + 1
                              );
                            }}
                          >
                            <IconPlus />
                          </ActionIcon>
                        )}

                        {!viewMode && (
                          <ActionIcon
                            variant="subtle"
                            c={"red"}
                            onClick={() => {
                              form.removeListItem(
                                "trainingEligibilities",
                                index
                              );
                            }}
                          >
                            <IconTrash />
                          </ActionIcon>
                        )}
                      </Flex>
                    )
                  )}
                </Accordion.Panel>
              </Accordion.Item>
            </Accordion>

            <Box mt={20}>
              {viewMode && (
                <Button size="lg" onClick={() => setViewMode(false)}>
                  {t("edit")}
                </Button>
              )}

              {!viewMode && (
                <>
                  <Button
                    disabled={!cForm?.isReady}
                    size="lg"
                    type="submit"
                    loading={updateCourse.isPending}
                  >
                    {t("submit")}
                  </Button>
                  <Button
                    ml={15}
                    size="lg"
                    onClick={() => setViewMode(true)}
                    variant="outline"
                  >
                    {t("cancel")}
                  </Button>
                </>
              )}
            </Box>
          </Box>
        </form>
      </FormProvider>
    </div>
  );
};

export default EditCourse;
