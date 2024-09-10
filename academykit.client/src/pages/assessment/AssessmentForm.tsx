import RichTextEditor from "@components/Ui/RichTextEditor/Index";
import { useAssessmentUtils } from "@hooks/useAssessmentUtils";
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
import { UseFormReturnType } from "@mantine/form";
import { IconPlus, IconTrash } from "@tabler/icons-react";
import { t } from "i18next";
import { useEffect, useState } from "react";

import { IAssessmentForm } from "./CreateAssessment";

function AssessmentForm({
  form,
  useFormContext,
}: {
  readonly form: UseFormReturnType<
    IAssessmentForm,
    (values: IAssessmentForm) => IAssessmentForm
  >;
  readonly useFormContext: () => UseFormReturnType<
    IAssessmentForm,
    (values: IAssessmentForm) => IAssessmentForm
  >;
}) {
  const [chooseMarkingType, setChooseMarkingType] = useState<
    "percentagePass" | "skills" | null
  >(null);

  const {
    getSkillDropdown,
    getDepartmentDropdown,
    getAssessmentDropdown,
    getGroupDropdown,
    getSkillAssessmentType,
    getTrainingDropdown,
  } = useAssessmentUtils();

  useEffect(() => {
    if (chooseMarkingType === "percentagePass") {
      form.setFieldValue("skillsCriteriaRequestModels", []);
    } else if (chooseMarkingType === "skills") {
      form.setFieldValue("passPercentage", null);
    }
  }, [chooseMarkingType]);

  return (
    <>
      <SimpleGrid
        cols={{ base: 1, sm: 2, lg: 3 }}
        spacing={{ base: 10, sm: "xl" }}
        verticalSpacing={{ base: "md", sm: "xl" }}
      >
        <TextInput
          autoFocus
          withAsterisk
          label={t("title")}
          placeholder={t("title_placeholder")}
          {...form.getInputProps("title")}
        />
        <DatePickerInput
          withAsterisk
          label={t("start_date")}
          placeholder={t("start_date")}
          minDate={new Date()}
          {...form.getInputProps("startDate")}
        />
        <DatePickerInput
          withAsterisk
          label={t("end_date")}
          placeholder={t("end_date")}
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
          placeholder={t("assessment_description")}
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
                  key={index + 1}
                  wrap={"wrap"}
                >
                  <Select
                    label={t("skill")}
                    placeholder={t("pick_value")}
                    data={getSkillDropdown() ?? []}
                    {...form.getInputProps(
                      `eligibilityCreationRequestModels.${index}.skill`
                    )}
                  />
                  <Select
                    label={t("role")}
                    placeholder={t("pick_value")}
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
                    placeholder={t("pick_value")}
                    data={getDepartmentDropdown() ?? []}
                    {...form.getInputProps(
                      `eligibilityCreationRequestModels.${index}.departmentId`
                    )}
                  />
                  <Select
                    label={t("group")}
                    placeholder={t("pick_value")}
                    data={getGroupDropdown() ?? []}
                    {...form.getInputProps(
                      `eligibilityCreationRequestModels.${index}.groupId`
                    )}
                  />
                  <Select
                    label={t("assessment")}
                    placeholder={t("pick_value")}
                    data={getAssessmentDropdown() ?? []}
                    {...form.getInputProps(
                      `eligibilityCreationRequestModels.${index}.assessmentId`
                    )}
                  />
                  <Select
                    label={t("training")}
                    placeholder={t("pick_value")}
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
                        form.values.eligibilityCreationRequestModels.length
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
          setChooseMarkingType(value as "percentagePass" | "skills" | null)
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
                  <Flex gap={10} key={index + 1} align={"flex-end"} mb={10}>
                    <Select
                      label={t("rule")}
                      placeholder={t("pick_value")}
                      data={getSkillAssessmentType() ?? []}
                      {...form.getInputProps(
                        `skillsCriteriaRequestModels.${index}.rule`
                      )}
                    />

                    <Select
                      label={t("skill")}
                      placeholder={t("pick_value")}
                      data={getSkillDropdown() ?? []}
                      {...form.getInputProps(
                        `skillsCriteriaRequestModels.${index}.skill`
                      )}
                    />

                    <NumberInput
                      label={t("percentage")}
                      min={0}
                      stepHoldDelay={500}
                      stepHoldInterval={(t) => Math.max(1000 / t ** 2, 25)}
                      placeholder={t("percentage_placeholder")}
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

                    {form.values.skillsCriteriaRequestModels.length > 1 && (
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
                    )}
                  </Flex>
                )
              )}
            </Accordion.Panel>
          </Accordion.Item>
        </Accordion>
      )}

      {chooseMarkingType !== null && chooseMarkingType === "percentagePass" && (
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
    </>
  );
}

export default AssessmentForm;
