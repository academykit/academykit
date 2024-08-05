import {
  Anchor,
  Box,
  Button,
  Center,
  Container,
  Group,
  Text,
  Title,
} from '@mantine/core';
import { useTranslation } from 'react-i18next';
import { Link, useParams, useSearchParams } from 'react-router-dom';

const ZoomMettingMessage = () => {
  const [searchParams] = useSearchParams();
  const { t } = useTranslation();
  const { courseId, lessonId } = useParams();
  const s = searchParams.get('s');
  const e = searchParams.get('e');

  if (s === '4') {
    return (
      <Center pt="20%">
        <Container>
          <Title ta="center">{t('error_while_joining')}</Title>
          <Text ta="center">{e}</Text>
          <Box my={20} ml={180}>
            <Group>
              <Anchor
                size={'xs'}
                component={Link}
                to={`/classes/${courseId}/${lessonId}/description`}
                c={'dimmed'}
              >
                <Button>{t('back_course')}</Button>
              </Anchor>
              <Anchor
                size={'xs'}
                component={Link}
                to={'#'}
                c={'dimmed'}
                target="_blank"
              >
                <Button
                  variant="outline"
                  component="a"
                  href={`/meet.html?l=${lessonId}&c=${courseId}`}
                >
                  {t('rejoin')}
                </Button>
              </Anchor>
            </Group>
          </Box>
        </Container>
      </Center>
    );
  }

  if (s === '1') {
    return (
      <Center pt="20%">
        <Container>
          <Title ta="center">{t('thank_for_attending_meeting')}</Title>
          <Text size={'xl'} ta="center">
            {t('left_meeting')}
          </Text>
          <Box my={20}>
            <Group style={{ display: 'flex', justifyContent: 'center' }}>
              <Anchor
                size={'xs'}
                component={Link}
                to={`/classes/${courseId}/${lessonId}/description`}
                c={'dimmed'}
              >
                <Button>{t('back_course')}</Button>
              </Anchor>
              <Anchor
                size={'xs'}
                component={Link}
                to={'#'}
                c={'dimmed'}
                target="_blank"
              >
                <Button
                  variant="outline"
                  component="a"
                  href={`/meet.html?l=${lessonId}&c=${courseId}`}
                >
                  {t('rejoin')}
                </Button>
              </Anchor>
            </Group>
          </Box>
        </Container>
      </Center>
    );
  }

  return (
    <Center pt={'20%'}>
      <Container>
        <Title>{t('meeting_end')}</Title>
        <Text size={'xl'} ta="center">
          {t('meeting_end_desc')}
        </Text>
        <Box my={20} ml={125}>
          <Group>
            <Anchor
              size={'xs'}
              component={Link}
              to={`/classes/${courseId}/${lessonId}/description`}
              c={'dimmed'}
            >
              <Button>{t('back_course')}</Button>
            </Anchor>
            <Anchor size={'xs'} component={Link} to={'#'} c={'dimmed'}></Anchor>
          </Group>
        </Box>
      </Container>
    </Center>
  );
};

export default ZoomMettingMessage;
