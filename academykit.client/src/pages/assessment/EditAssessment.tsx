import useFormErrorHooks from "@hooks/useFormErrorHooks";
import { Box, Button } from "@mantine/core";
import { createFormContext, yupResolver } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import RoutePath from "@utils/routeConstants";
import {
  useGetSingleAssessment,
  useUpdateAssessment,
} from "@utils/services/assessmentService";
import errorType from "@utils/services/axiosError";
import { t } from "i18next";
import moment from "moment";
import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import AssessmentForm from "./AssessmentForm";
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
  const params = useParams();
  const assessmentData = useGetSingleAssessment(params.id as string);
  const [chooseMarkingType, setChooseMarkingType] = useState<
    "percentagePass" | "skills" | null
  >(null);

  const updateAssessment = useUpdateAssessment(params.id as string);

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
        duration: assessmentData.data?.duration / 60, // show in minutes
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
    <Box mt={10}>
      <FormProvider form={form}>
        <form onSubmit={form.onSubmit(onSubmit)}>
          <AssessmentForm
            form={form}
            useFormContext={useFormContext}
            chooseMarkingType={chooseMarkingType}
            setChooseMarkingType={setChooseMarkingType}
          />
          <Button mt={30} type="submit" loading={updateAssessment.isPending}>
            {t("submit")}
          </Button>
        </form>
      </FormProvider>
    </Box>
  );
};

export default EditAssessment;
