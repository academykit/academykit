import Breadcrumb from "@components/Ui/BreadCrumb";
import useFormErrorHooks from "@hooks/useFormErrorHooks";
import { Box, Button } from "@mantine/core";
import { createFormContext, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import RoutePath from "@utils/routeConstants";
import { usePostAssessment } from "@utils/services/assessmentService";
import errorType from "@utils/services/axiosError";
import { t } from "i18next";
import moment from "moment";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import AssessmentForm from "./AssessmentForm";
import schema from "./component/AssessmentFormSchema";

export type SkillsCriteriaRequestModels = Array<{
  rule: string;
  skill: string;
  percentage: number;
}>;
export type EligibilityCriteriaRequestModels = Array<{
  skill: string;
  role: string;
  departmentId: string;
  groupId: string;
  assessmentId: string;
  trainingId: string;
}>;

export interface IAssessmentForm {
  title: string;
  description: string;
  retakes: number;
  startDate: Date | null;
  endDate: Date | null;
  duration: number;
  weightage: number;
  skillsCriteriaRequestModels: SkillsCriteriaRequestModels;
  eligibilityCreationRequestModels: EligibilityCriteriaRequestModels;
  passPercentage: number | null;
}

const [FormProvider, useFormContext, useForm] =
  createFormContext<IAssessmentForm>();

const CreateAssessment = () => {
  const navigate = useNavigate();
  const postAssessment = usePostAssessment();

  const [chooseMarkingType, setChooseMarkingType] = useState<
    "percentagePass" | "skills" | null
  >(null);

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
    if (chooseMarkingType === "percentagePass") {
      form.setFieldValue("skillsCriteriaRequestModels", []);
    } else if (chooseMarkingType === "skills") {
      form.setFieldValue("passPercentage", null);
    }
  }, [chooseMarkingType]);

  const onSubmit = async (values: typeof form.values) => {
    try {
      const data = {
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
          (skill) => {
            return {
              skillAssessmentRule: Number(skill.rule),
              skillId: skill.skill,
              percentage: skill.percentage,
            };
          }
        ),
        eligibilityCreationRequestModels:
          values.eligibilityCreationRequestModels.map((eligibility) => {
            return {
              skillId: eligibility.skill,
              role: Number(eligibility.role),
              departmentId: eligibility.departmentId,
              groupId: eligibility.groupId,
              assessmentId: eligibility.assessmentId,
              trainingId: eligibility.trainingId,
            };
          }),
      };

      const response = await postAssessment.mutateAsync(data);

      form.reset();

      showNotification({
        title: t("successful"),
        message: t("assessment_created_success"),
      });

      navigate(RoutePath.manageAssessment.question(response.data.slug).route);
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
      <Breadcrumb />

      <Box mt={10}>
        <FormProvider form={form}>
          <form onSubmit={form.onSubmit(onSubmit)}>
            <AssessmentForm
              form={form}
              useFormContext={useFormContext}
              chooseMarkingType={chooseMarkingType}
              setChooseMarkingType={setChooseMarkingType}
            />
            <Button mt={30} type="submit" loading={postAssessment.isPending}>
              {t("submit")}
            </Button>
          </form>
        </FormProvider>
      </Box>
    </>
  );
};

export default CreateAssessment;
