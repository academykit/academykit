import { Box, Button, Group, Title } from "@mantine/core";
import RoutePath from "@utils/routeConstants";
import { Link } from "react-router-dom";

const FeedbackDetails = ({
  name,
  id,
  hasFeedbackSubmitted,
}: {
  name: string;
  id: string;
  hasFeedbackSubmitted: boolean;
}) => {
  return (
    <Group sx={{ flexDirection: "column" }}>
      <Title>{name}</Title>
      {!hasFeedbackSubmitted ? (
        <Button component={Link} to={RoutePath.feedback.details(id).route}>
          Give Feedback
        </Button>
      ) : (
        <Button component={Link} to={RoutePath.feedback.myDetails(id).route}>
          View Feedback
        </Button>
      )}
    </Group>
  );
};

export default FeedbackDetails;
