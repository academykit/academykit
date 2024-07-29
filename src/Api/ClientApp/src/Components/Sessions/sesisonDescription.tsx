import {
  AspectRatio,
  Badge,
  Box,
  Button,
  Center,
  Container,
  Group,
  Image,
  List,
  Text,
  ThemeIcon,
  Title,
} from '@mantine/core';
import { IconCheck } from '@tabler/icons-react';
import RoutePath from '@utils/routeConstants';
import { Link } from 'react-router-dom';
import classes from './styles/session.module.css';

const image = 'https://ui.mantine.dev/_next/static/media/image.9a65bd94.svg';

const SessionDescription = () => {
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
                c="green"
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
            <Text mt={'md'} fw={750}>
              Description
            </Text>
            <Text c="dimmed" mt="sm">
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
                <Link to={RoutePath.classes + '/1'}>
                  <Button radius="xl" size="md" className={classes.control}>
                    Enroll Now Rs.(500)
                  </Button>
                </Link>
              </Group>
            </Center>
            <Text ta="center" fw={200} style={{ marginBottom: '50px' }}>
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
