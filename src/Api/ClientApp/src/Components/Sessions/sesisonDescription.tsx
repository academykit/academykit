import CourseContent from "@components/Course/CourseDescription/CourseContent/CourseContent";
import UserShortProfile from "@components/UserShortProfile";
import {
  AspectRatio,
  Badge,
  Box,
  Button,
  Center,
  Container,
  createStyles,
  Group,
  Image,
  List,
  Text,
  ThemeIcon,
  Title,
} from "@mantine/core";
import { IconCheck } from "@tabler/icons";
import { UserRole } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import { Link } from "react-router-dom";
const image = "https://ui.mantine.dev/_next/static/media/image.9a65bd94.svg";

const useStyles = createStyles((theme) => ({
  wrapper: {
    marginLeft: 40,
    marginRight: 40,
  },
  inner: {
    display: "flex",
    justifyContent: "space-between",
    paddingTop: theme.spacing.xl,
    paddingBottom: theme.spacing.xl * 4,
    [theme.fn.smallerThan("sm")]: {
      flexDirection: "column-reverse",
    },
  },

  content: {
    width: "60%",
    marginRight: theme.spacing.xl * 3,

    [theme.fn.smallerThan("lg")]: {
      width: "50%",
    },
    [theme.fn.smallerThan("sm")]: {
      width: "100%",
    },
  },

  title: {
    color: theme.colorScheme === "dark" ? theme.white : theme.black,
    fontFamily: `Greycliff CF, ${theme.fontFamily}`,
    fontSize: 42,
    lineHeight: 1.2,
    fontWeight: 800,
    position: "relative",

    [theme.fn.smallerThan("xs")]: {
      fontSize: 28,
    },
  },

  control: {
    [theme.fn.smallerThan("xs")]: {
      flex: 1,
    },
  },

  aside: {
    width: "40%",
    [theme.fn.smallerThan("lg")]: {
      width: "50%",
    },
    [theme.fn.smallerThan("sm")]: {
      width: "100%",
    },
  },

  highlight: {
    position: "relative",
    backgroundColor: theme.fn.variant({
      variant: "light",
      color: theme.primaryColor,
    }).background,
    borderRadius: theme.radius.sm,
    padding: "4px 12px",
  },
  CourseContentSmall: {
    display: "none",
    [theme.fn.smallerThan("sm")]: {
      display: "block",
    },
  },

  CourseContentLarge: {
    display: "block",
    [theme.fn.smallerThan("sm")]: {
      display: "none",
    },
  },
  innerContent: {
    display: "flex",
  },
}));

const SessionDescription = () => {
  const { classes } = useStyles();
  return (
    <div>
      <Container fluid>
        <div className={classes.inner}>
          <div className={classes.content}>
            <div className={classes.innerContent}>
              <Title className={classes.title}>
                A modern React components library
              </Title>
              <Badge
                // className={classes.BadgeWrapperPrice}
                color="green"
                variant="outline"
              >
                Rs 500
              </Badge>
            </div>
            <Group my={4}>
              {/* <UserShortProfile
               
                size={60}
              /> */}
            </Group>
            <Text mt={"md"} weight={750}>
              Description
            </Text>
            <Text color="dimmed" mt="sm">
              Build fully functional accessible web applications faster than
              ever – Mantine includes more than 120 customizable components and
              hooks to cover you in any situation
            </Text>
            <List
              mt={30}
              spacing="sm"
              size="sm"
              icon={
                <ThemeIcon size={20} radius="xl">
                  <IconCheck size={12} stroke={1.5} />
                </ThemeIcon>
              }
            >
              <List.Item>
                <b>TypeScript based</b> – build type safe applications, all
                components and hooks export types
              </List.Item>
              <List.Item>
                <b>Free and open source</b> – all packages have MIT license, you
                can use Mantine in any project
              </List.Item>
              <List.Item>
                <b>No annoying focus ring</b> – focus ring will appear only when
                user navigates with keyboard
              </List.Item>
            </List>
          </div>
          <div className={classes.aside}>
            <AspectRatio ratio={16 / 9} mx="auto">
              <Image src={image} />
            </AspectRatio>
            <Center>
              <Group my={30}>
                <Link to={RoutePath.classes + "/1"}>
                  <Button radius="xl" size="md" className={classes.control}>
                    Enroll Now Rs.(500)
                  </Button>
                </Link>
              </Group>
            </Center>
            <Text align="center" weight={200} sx={{ marginBottom: "50px" }}>
              Pay here to enroll in this live session
            </Text>
          </div>
        </div>
        <Box className={classes.CourseContentSmall}>
          {/* <CourseContent /> */}
        </Box>
      </Container>
    </div>
  );
};
export default SessionDescription;
