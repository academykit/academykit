import { List, Text } from "@mantine/core";
import { IconCheck, IconX } from "@tabler/icons-react";

type CourseEligibilityProps = {
  index: number;
  eligibility: number;
  course: any;
};

const CourseEligibility = ({
  index,
  eligibility,
  course,
}: CourseEligibilityProps) => {
  return (
    <List.Item
      key={index}
      icon={
        // show eligibility status icon only if the user is not admin or super-admin
        // and it not the owner of the assessment
        course?.isEligible && (
          <>{eligibility ? <IconCheck size={18} /> : <IconX size={18} />}</>
        )
      }
    >
      <Text lineClamp={1}>{`Must`}</Text>
    </List.Item>
  );
};

export default CourseEligibility;
