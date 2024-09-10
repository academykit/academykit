import { useTranslation } from "react-i18next";
import * as Yup from "yup";

const schema = () => {
  const { t } = useTranslation();

  return Yup.object().shape({
    title: Yup.string()
      .trim()
      .required(t("assessment_title_required") as string),
    startDate: Yup.date()
      .required(t("start_date_required") as string)
      .typeError(t("start_date_required") as string),
    endDate: Yup.date()
      .required(t("end_date_required") as string)
      .typeError(t("end_date_required") as string)
      .min(
        Yup.ref("startDate"),
        t("end_date_must_be_after_start_date") as string
      ),
    retakes: Yup.number()
      .min(0, t("value_must_be_at_least_0") as string)
      .required(t("retake_required") as string),
    weightage: Yup.number()
      .min(1, t("value_must_be_at_least_1") as string)
      .required(t("weightage_required") as string),
    passPercentage: Yup.number()
      .min(1, t("value_must_be_at_least_1") as string)
      .nullable(),
    duration: Yup.number()
      .min(1, t("value_must_be_at_least_1") as string)
      .required(t("duration_required") as string),
    skillsCriteriaRequestModels: Yup.array()
      .of(
        Yup.object().shape({
          rule: Yup.string().nullable(),
          skill: Yup.string().nullable(),
          percentage: Yup.number()
            .min(0, t("value_must_be_at_least_0") as string)
            .max(100, t("percentage_must_be_at_most_100") as string),
        })
      )
      .optional(),
  });
};

export default schema;
