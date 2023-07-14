import {
  Badge,
  Card,
  createStyles,
  Grid,
  Group,
  Image,
  Text,
} from '@mantine/core';

function SessionCard() {
  const useStyles = createStyles((theme) => ({
    BadgeWrapperLive: {
      position: 'absolute',
      top: 5,
      color: 'red',
    },
    BadgeWrapperEnrolled: {
      position: 'absolute',
      top: 5,
      left: '25%',
      marginLeft: '5px',
      [theme.fn.smallerThan('lg')]: {
        marginLeft: '35px',
      },
      [theme.fn.smallerThan('sm')]: {
        marginLeft: '30px',
      },
    },
  }));
  const { classes, cx } = useStyles();
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
        color="green"
        variant="filled"
      >
        Enrolled
      </Badge>

      <Text weight={500} size="lg" mt="md">
        Ielts master class
      </Text>

      <Group position="apart" mt="md" mb="xs">
        <Text weight={500}>Duration: 1hr</Text>
        <Text weight={500}>Starts in 1hr</Text>
      </Group>

      <Text mt="xs" color="dimmed" size="sm">
        Balen name
      </Text>
    </Card>
  );
}
export default SessionCard;
