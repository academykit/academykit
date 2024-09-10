import { SkillAssessmentRule } from "@utils/enums";
import queryStringGenerator from "@utils/queryStringGenerator";
import { useDepartmentSetting } from "@utils/services/adminService";
import { useAssessments } from "@utils/services/assessmentService";
import { useCourse } from "@utils/services/courseService";
import { useGroups } from "@utils/services/groupService";
import { useSkills } from "@utils/services/skillService";
import { t } from "i18next";

export function useDropdownUtils() {
  const skillData = useSkills(queryStringGenerator({ size: 1000 }));
  const getDepartment = useDepartmentSetting(
    queryStringGenerator({ size: 1000 })
  );
  const getGroups = useGroups(queryStringGenerator({ size: 1000 }));
  const getAssessments = useAssessments(queryStringGenerator({ size: 1000 }));
  const getTrainings = useCourse(queryStringGenerator({ size: 1000 }));

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

  const getSkillDropdown = () => {
    return skillData.data?.items.map((skill) => ({
      value: skill.id,
      label: skill.skillName,
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

  const getSkillAssessmentType = () => {
    return Object.entries(SkillAssessmentRule)
      .splice(0, Object.entries(SkillAssessmentRule).length / 2)
      .map(([key, value]) => ({
        value: key,
        label: t(value.toString()),
      }));
  };

  return {
    getDepartmentDropdown,
    getGroupDropdown,
    getSkillDropdown,
    getAssessmentDropdown,
    getTrainingDropdown,
    getSkillAssessmentType,
  };
}
