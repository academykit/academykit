import RichTextEditor from "@components/Ui/RichTextEditor/Index";
import useFormErrorHooks from "@hooks/useFormErrorHooks";
import {
  Accordion,
  ActionIcon,
  Box,
  Button,
  Flex,
  Group,
  NumberInput,
  Radio,
  Select,
  SimpleGrid,
  Text,
  TextInput,
} from "@mantine/core";
import { DatePickerInput } from "@mantine/dates";
import { createFormContext, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import { IconPlus, IconTrash } from "@tabler/icons-react";
import { SkillAssessmentRule } from "@utils/enums";
import queryStringGenerator from "@utils/queryStringGenerator";
import RoutePath from "@utils/routeConstants";
import { useDepartmentSetting } from "@utils/services/adminService";
import {
  useAssessments,
  useGetSingleAssessment,
  useUpdateAssessment,
} from "@utils/services/assessmentService";
import errorType from "@utils/services/axiosError";
import { useCourse } from "@utils/services/courseService";
import { useGroups } from "@utils/services/groupService";
import { useSkills } from "@utils/services/skillService";
import { t } from "i18next";
import moment from "moment";
import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import {
  EligibilityCriteriaRequestModels,
  IAssessmentForm,
  SkillsCriteriaRequestModels,
} from "./CreateAssessment";
import schema from "./component/AssessmentFormSchema";

const [FormProvider, useFormContext, useForm] =
  createFormContext<IAssessmentForm>();

const EditAssessment = () => {
  const navigate = useNavigate();
  const skillData = useSkills(queryStringGenerator({ size: 1000 }));
  const params = useParams();
  const assessmentData = useGetSingleAssessment(params.id as string);
  const updateAssessment = useUpdateAssessment(params.id as string);
  const getDepartment = useDepartmentSetting(
    queryStringGenerator({ size: 1000 })
  );
  const getGroups = useGroups(queryStringGenerator({ size: 1000 }));
  const getAssessments = useAssessments(queryStringGenerator({ size: 1000 }));
  const getTrainings = useCourse(queryStringGenerator({ size: 1000 }));

  const [chooseMarkingType, setChooseMarkingType] = useState<
    "percentagePass" | "skills" | null
  >(null);

  const getDepartmentDropdown = () => {
    return getDepartment.data?.items.map((department) => ({
      value: department.id,
      label: department.name,
    }));
  };

  const getGroupDropdown = () => {
    return getGroups.data?.data.items.map((group) => ({
      value: group.id,
      label: group.name,
    }));
  };

  const getAssessmentDropdown = () => {
    return getAssessments.data?.items.map((assessment) => ({
      value: assessment.id,
      label: assessment.title,
    }));
  };

  const getTrainingDropdown = () => {
    return getTrainings.data?.items.map((training) => ({
      value: training.id,
      label: training.name,
    }));
  };

  const getSkillDropdown = () => {
    return skillData.data?.items.map((skill) => ({
      value: skill.id,
      label: skill.skillName,
    }));
  };

  const getSkillAssessmentType = () => {
    return Object.entries(SkillAssessmentRule)
      .splice(0, Object.entries(SkillAssessmentRule).length / 2)
      .map(([key, value]) => ({
        value: key,
        label: t(value.toString()),
      }));
  };

  const form = useForm({
    initialValues: {
      title: "",
      startDate: null,
      endDate: null,
      retakes: 0,
      weightage: 1,
      duration: 1,
      description: "",
      passPercentage: null,
      skillsCriteriaRequestModels: [],
      eligibilityCreationRequestModels: [],
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  useEffect(() => {
    if (assessmentData.data) {
      const eligibility: EligibilityCriteriaRequestModels =
        assessmentData.data?.eligibilityCreationRequestModels.map((item) => {
          return {
            skill: item.skillId,
            role: item.role.toString(),
            departmentId: item.departmentId,
            groupId: item.groupId,
            assessmentId: item.assessmentId,
            trainingId: item.trainingId,
          };
        });

      const skills: SkillsCriteriaRequestModels =
        assessmentData.data?.skillsCriteriaRequestModels.map((item) => {
          return {
            rule: item.skillAssessmentRule.toString(),
            skill: item.skillId,
            percentage: item.percentage,
          };
        });

      form.setValues({
        title: assessmentData.data?.title ?? "",
        retakes: assessmentData.data?.retakes ?? 0,
        weightage: assessmentData.data?.weightage ?? 0,
        duration: assessmentData.data?.duration / 60 ?? 0, // show in minutes
        description: assessmentData.data?.description ?? "",
        eligibilityCreationRequestModels: eligibility,
        skillsCriteriaRequestModels: skills,
        startDate: moment(assessmentData.data.startDate).toDate(),
        endDate: moment(assessmentData.data.endDate).toDate(),
        passPercentage: assessmentData.data?.passPercentage,
      });

      if (assessmentData.data?.passPercentage > 0) {
        setChooseMarkingType("percentagePass");
      } else if (assessmentData.data?.skillsCriteriaRequestModels.length > 0) {
        setChooseMarkingType("skills");
      }
    }
  }, [assessmentData.isSuccess]);

  const onSubmit = async (values: typeof form.values) => {
    try {
      const baseData = {
        ...values,
        startDate: moment(values.startDate)
          .add(5, "hour")
          .add(45, "minute")
          .toDate(),
        endDate: moment(values.endDate)
          .add(5, "hour")
          .add(45, "minute")
          .toDate(),
        skillsCriteriaRequestModels: values.skillsCriteriaRequestModels.map(
          (skill) => ({
            skillAssessmentRule: Number(skill.rule),
            skillId: skill.skill,
            percentage: skill.percentage,
          })
        ),
        eligibilityCreationRequestModels:
          values.eligibilityCreationRequestModels.map((eligibility) => ({
            skillId: eligibility.skill,
            role: Number(eligibility.role),
            departmentId: eligibility.departmentId,
            groupId: eligibility.groupId,
            assessmentId: eligibility.assessmentId,
            trainingId: eligibility.trainingId,
          })),
      };

      let data: typeof baseData;

      if (
        chooseMarkingType === "skills" &&
        form.values.skillsCriteriaRequestModels.length > 0
      ) {
        data = {
          ...baseData,
          passPercentage: null,
        };
      } else if (
        chooseMarkingType === "percentagePass" &&
        form?.values?.passPercentage &&
        form?.values?.passPercentage > 0
      ) {
        data = {
          ...baseData,
          skillsCriteriaRequestModels: [],
        };
      } else {
        data = {
          ...baseData,
        };
      }

      await updateAssessment.mutateAsync({ id: params.id as string, data });

      showNotification({
        title: t("successful"),
        message: t("assessment_update_success"),
      });

      navigate(RoutePath.manageAssessment.question(params.id).route);
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: "red",
      });
    }
  };

  return (
    <>
      <Box mt={10}>
        <FormProvider form={form}>
          <form onSubmit={form.onSubmit(onSubmit)}>
            <SimpleGrid
              cols={{ base: 1, sm: 2, lg: 3 }}
              spacing={{ base: 10, sm: "xl" }}
              verticalSpacing={{ base: "md", sm: "xl" }}
            >
              <TextInput
                withAsterisk
                label={t("title")}
                placeholder={t("title_placeholder") as string}
                {...form.getInputProps("title")}
              />
              <DatePickerInput
                withAsterisk
                label={t("start_date")}
                placeholder={t("start_date") as string}
                minDate={new Date()}
                {...form.getInputProps("startDate")}
              />
              <DatePickerInput
                withAsterisk
                label={t("end_date")}
                placeholder={t("end_date") as string}
                minDate={form.values.startDate ?? new Date()}
                {...form.getInputProps("endDate")}
              />
              <NumberInput
                withAsterisk
                label={t("retake")}
                placeholder="Input placeholder"
                min={0}
                stepHoldDelay={500}
                stepHoldInterval={(t) => Math.max(1000 / t ** 2, 25)}
                {...form.getInputProps("retakes")}
              />
              <NumberInput
                withAsterisk
                label={t("weightage")}
                placeholder="Input placeholder"
                min={1}
                stepHoldDelay={500}
                stepHoldInterval={(t) => Math.max(1000 / t ** 2, 25)}
                {...form.getInputProps("weightage")}
              />
              <NumberInput
                withAsterisk
                label={t("duration")}
                placeholder="Input placeholder"
                min={1}
                stepHoldDelay={500}
                stepHoldInterval={(t) => Math.max(1000 / t ** 2, 25)}
                {...form.getInputProps("duration")}
              />
            </SimpleGrid>

            <Box my={20}>
              <Text>{t("description")}</Text>
              <RichTextEditor
                placeholder={t("assessment_description") as string}
                formContext={useFormContext}
              />
            </Box>

            <Accordion defaultValue="Eligibility">
              <Accordion.Item value="Eligibility">
                <Accordion.Control>{t("eligibility")}</Accordion.Control>
                <Accordion.Panel>
                  {form.values.eligibilityCreationRequestModels.length < 1 && (
                    <Button
                      onClick={() => {
                        form.insertListItem(
                          "eligibilityCreationRequestModels",
                          {
                            skill: "",
                            role: "",
                            departmentId: "",
                            groupId: "",
                            completedAssessmentId: "",
                            trainingId: "",
                          },
                          0
                        );
                      }}
                    >
                      {t("add_eligibility_criteria")}
                    </Button>
                  )}

                  {form.values.eligibilityCreationRequestModels.map(
                    (_criteria, index) => (
                      <Flex
                        mb={10}
                        gap={10}
                        align={"flex-end"}
                        wrap={"wrap"}
                        key={index}
                      >
                        <Select
                          label={t("skill")}
                          placeholder={t("pick_value") as string}
                          data={getSkillDropdown() ?? []}
                          {...form.getInputProps(
                            `eligibilityCreationRequestModels.${index}.skill`
                          )}
                        />
                        <Select
                          label={t("role")}
                          placeholder={t("pick_value") as string}
                          data={[
                            {
                              value: "3",
                              label: "Trainer",
                            },
                            {
                              value: "4",
                              label: "Trainee",
                            },
                          ]}
                          {...form.getInputProps(
                            `eligibilityCreationRequestModels.${index}.role`
                          )}
                        />

                        <Select
                          label={t("department")}
                          placeholder={t("pick_value") as string}
                          data={getDepartmentDropdown() ?? []}
                          {...form.getInputProps(
                            `eligibilityCreationRequestModels.${index}.departmentId`
                          )}
                        />
                        <Select
                          label={t("group")}
                          placeholder={t("pick_value") as string}
                          data={getGroupDropdown() ?? []}
                          {...form.getInputProps(
                            `eligibilityCreationRequestModels.${index}.groupId`
                          )}
                        />
                        <Select
                          label={t("assessment")}
                          placeholder={t("pick_value") as string}
                          data={getAssessmentDropdown() ?? []}
                          {...form.getInputProps(
                            `eligibilityCreationRequestModels.${index}.assessmentId`
                          )}
                        />
                        <Select
                          label={t("training")}
                          placeholder={t("pick_value") as string}
                          data={getTrainingDropdown() ?? []}
                          {...form.getInputProps(
                            `eligibilityCreationRequestModels.${index}.trainingId`
                          )}
                        />

                        <ActionIcon
                          variant="subtle"
                          onClick={() => {
                            form.insertListItem(
                              "eligibilityCreationRequestModels",
                              {
                                skill: "",
                                role: "",
                                departmentId: "",
                                groupId: "",
                                completedAssessmentId: "",
                                trainingId: "",
                              },
                              // add to the end of the list
                              form.values.eligibilityCreationRequestModels
                                .length
                            );
                          }}
                        >
                          <IconPlus />
                        </ActionIcon>
                        <ActionIcon
                          variant="subtle"
                          c={"red"}
                          onClick={() => {
                            form.removeListItem(
                              "eligibilityCreationRequestModels",
                              index
                            );
                          }}
                        >
                          <IconTrash />
                        </ActionIcon>
                      </Flex>
                    )
                  )}
                </Accordion.Panel>
              </Accordion.Item>
            </Accordion>

            <Radio.Group
              name="chooseMarkingType"
              mt="lg"
              label={t("choose_grade")}
              value={chooseMarkingType}
              onChange={(value) =>
                setChooseMarkingType(
                  value as "percentagePass" | "skills" | null
                )
              }
            >
              <Group mt="xs">
                <Radio value="percentagePass" label="Percentage" />
                <Radio value="skills" label={t("Skills")} />
              </Group>
            </Radio.Group>

            {chooseMarkingType !== null && chooseMarkingType === "skills" && (
              <Accordion defaultValue="Skill" mt="lg">
                <Accordion.Item value="Skill">
                  <Accordion.Control>{t("skill_criteria")}</Accordion.Control>
                  <Accordion.Panel>
                    {form.values.skillsCriteriaRequestModels.length === 0 && (
                      <Button
                        onClick={() => {
                          form.insertListItem("skillsCriteriaRequestModels", {
                            rule: "",
                            skill: "",
                            percentage: 0,
                          });
                        }}
                      >
                        {t("add_skill_criteria")}
                      </Button>
                    )}

                    {form.values.skillsCriteriaRequestModels.map(
                      (_criteria, index) => (
                        <Flex gap={10} key={index} align={"flex-end"} mb={10}>
                          <Select
                            label={t("rule")}
                            placeholder={t("pick_value") as string}
                            data={getSkillAssessmentType() ?? []}
                            {...form.getInputProps(
                              `skillsCriteriaRequestModels.${index}.rule`
                            )}
                          />

                          <Select
                            label={t("skill")}
                            placeholder={t("pick_value") as string}
                            data={getSkillDropdown() ?? []}
                            {...form.getInputProps(
                              `skillsCriteriaRequestModels.${index}.skill`
                            )}
                          />

                          <NumberInput
                            label={t("percentage")}
                            min={0}
                            stepHoldDelay={500}
                            stepHoldInterval={(t) =>
                              Math.max(1000 / t ** 2, 25)
                            }
                            placeholder={t("percentage_placeholder") as string}
                            {...form.getInputProps(
                              `skillsCriteriaRequestModels.${index}.percentage`
                            )}
                          />

                          <ActionIcon
                            variant="subtle"
                            onClick={() => {
                              form.insertListItem(
                                "skillsCriteriaRequestModels",
                                { rule: "", skill: "", percentage: 0 },
                                form.values.skillsCriteriaRequestModels.length // add to the end of the list
                              );
                            }}
                          >
                            <IconPlus />
                          </ActionIcon>

                          <ActionIcon
                            variant="subtle"
                            c={"red"}
                            onClick={() => {
                              form.removeListItem(
                                "skillsCriteriaRequestModels",
                                index
                              );
                            }}
                          >
                            <IconTrash />
                          </ActionIcon>
                        </Flex>
                      )
                    )}
                  </Accordion.Panel>
                </Accordion.Item>
              </Accordion>
            )}

            {chooseMarkingType !== null &&
              chooseMarkingType === "percentagePass" && (
                <NumberInput
                  mt="lg"
                  maw={200}
                  label={t("pass_percentage")}
                  min={1}
                  max={100}
                  stepHoldDelay={500}
                  stepHoldInterval={(t) => Math.max(1000 / t ** 2, 25)}
                  {...form.getInputProps("passPercentage")}
                />
              )}

            <Button mt={30} type="submit" loading={updateAssessment.isPending}>
              {t("submit")}
            </Button>
          </form>
        </FormProvider>
      </Box>
    </>
  );
};

export default EditAssessment;
