import { Badge, Card, Group, Image, Text } from "@mantine/core";
import classes from "./styles/session.module.css";

function SessionCard() {
  return (
    <Card
      shadow="sm"
      p="xl"
      component="a"
      href="https://www.youtube.com/watch?v=dQw4w9WgXcQ"
      target="_blank"
    >
      <Card.Section>
        <Image
          src="https://images.unsplash.com/photo-1579227114347-15d08fc37cae?ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&ixlib=rb-1.2.1&auto=format&fit=crop&w=2550&q=80"
          height={160}
          alt="No way!"
        />
      </Card.Section>
      <Badge color="red" variant="dot" className={classes.BadgeWrapperLive}>
        Live
      </Badge>

      <Badge
        className={classes.BadgeWrapperEnrolled}
        c="green"
        variant="filled"
      >
        Enrolled
      </Badge>

      <Text fw={500} size="lg" mt="md">
        React master class
      </Text>

      <Group justify="space-between" mt="md" mb="xs">
        <Text fw={500}>Duration: 1hr</Text>
        <Text fw={500}>Starts in 1hr</Text>
      </Group>

      <Text mt="xs" c="dimmed" size="sm">
        User name
      </Text>
    </Card>
  );
}
export default SessionCard;
