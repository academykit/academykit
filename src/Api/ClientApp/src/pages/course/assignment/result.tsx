import TextViewer from '@components/Ui/RichTextViewer';
import UserShortProfile from '@components/UserShortProfile';
import useAuth from '@hooks/useAuth';
import {
  Button,
  Card,
  Container,
  Group,
  Loader,
  Modal,
  Paper,
  Text,
  Title,
} from '@mantine/core';
import { useToggle } from '@mantine/hooks';
import { QuestionType, UserRole } from '@utils/enums';
import { useAssignmentReview } from '@utils/services/assignmentService';
import cx from 'clsx';
import { useTranslation } from 'react-i18next';
import { useNavigate, useParams } from 'react-router-dom';
import classes from '../styles/radioType.module.css';
import AssignmentReviewForm from './Component/AssignmentReviewForm';

const AssignmentResult = () => {
  const [showReviewBox, setShowReviewBox] = useToggle();
  const { id, studentId } = useParams();
  const navigate = useNavigate();
  const auth = useAuth();
  const { t } = useTranslation();
  const getAssignment = useAssignmentReview(id as string, studentId as string);

  if (getAssignment.isError) {
    throw getAssignment.error;
  }
  if (getAssignment.isLoading) {
    return <Loader />;
  }
  return (
    <Container>
      <Modal
        opened={showReviewBox}
        onClose={() => setShowReviewBox()}
        title={
          getAssignment.data?.assignmentReview?.review
            ? t('edit_assignment_review')
            : t('review_assignment')
        }
      >
        {showReviewBox && (
          <AssignmentReviewForm
            reviewId={getAssignment.data?.assignmentReview?.id}
            closeModal={() => setShowReviewBox()}
            edit={!!getAssignment.data.assignmentReview}
            editData={getAssignment.data}
          />
        )}
      </Modal>
      {getAssignment.data.assignments.map((x) => (
        <Card key={x.id} shadow="sm" my={10} withBorder>
          <Title>{x.name}</Title>
          {x.description && (
            <>
              <Text mt={15} fw="bold">
                {t('description')}
              </Text>
              <TextViewer content={x.description}></TextViewer>
            </>
          )}

          {x.hints && (
            <>
              <Text fw="bold">{t('hint')}</Text>
              <TextViewer content={x.hints} />
            </>
          )}
          {x.type === QuestionType.Subjective ? (
            <>
              <Text fw="bold">{t('answers')}</Text>
              <TextViewer content={x.answer ?? ''} />
            </>
          ) : (
            x.assignmentQuestionOptions &&
            x.assignmentQuestionOptions.map((option) => (
              <Card
                key={option.id}
                shadow={'md'}
                my={10}
                p={10}
                className={cx({
                  [classes.active]: option.isSelected,
                  [classes.wrong]: !option.isCorrect && option.isSelected,
                  [classes.correct]: option.isCorrect,
                })}
              >
                <input type={'checkbox'} style={{ display: 'none' }} />
                <TextViewer
                  styles={{
                    root: {
                      border: 'none',
                    },
                  }}
                  content={option.option}
                ></TextViewer>
              </Card>
            ))
          )}
        </Card>
      ))}
      {getAssignment.data.assignmentReview && (
        <Paper p={20}>
          <Title>{t('review')}</Title>
          <Group>
            <UserShortProfile
              size={'md'}
              user={getAssignment.data?.assignmentReview?.teacher}
            />
            <Paper withBorder shadow={'xl'} px={40} py={20} mx={20}>
              <Group>
                <Text> {t('mark')}</Text>
                <Text c={'dimmed'}>
                  {getAssignment.data?.assignmentReview?.mark}/100
                </Text>
              </Group>
              <Group>
                <Text>{t('review')}</Text>
                <Text c={'dimmed'}>
                  {getAssignment.data?.assignmentReview?.review}
                </Text>
              </Group>
            </Paper>
          </Group>
        </Paper>
      )}

      <Group my={20}>
        {auth?.auth && Number(auth?.auth?.role) <= UserRole.Trainer && (
          <Button onClick={() => setShowReviewBox()}>
            {getAssignment.data?.assignmentReview?.review
              ? t('edit_review')
              : t('add_review')}
          </Button>
        )}
        <Button
          variant="outline"
          onClick={() => {
            navigate(-1);
          }}
        >
          {t('go_back_button')}
        </Button>
      </Group>
    </Container>
  );
};

export default AssignmentResult;
