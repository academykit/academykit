import AddAssignment from '@components/Course/Lesson/AddAssignment';
import AddDocument from '@components/Course/Lesson/AddDocument';
import AddExam from '@components/Course/Lesson/AddExam';
import AddFeedback from '@components/Course/Lesson/AddFeedback';
import AddLecture from '@components/Course/Lesson/AddLecture';
import AddMeeting from '@components/Course/Lesson/AddMeeting';
import AddPhysical from '@components/Course/Lesson/AddPhysical';
import { useSection } from '@context/SectionProvider';
import { Badge, Button, Tooltip } from '@mantine/core';
import { TFunction } from 'i18next';
import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';

const LessonAddList = ({
  sectionId,
  setAddLessonClick,
  t,
}: {
  setAddLessonClick: React.Dispatch<React.SetStateAction<boolean>>;
  t: TFunction;
  sectionId: string;
}) => {
  const [addState, setAddState] = React.useState<string>('');
  let returnDiv;
  switch (addState) {
    case 'lecture':
      returnDiv = (
        <AddLecture
          sectionId={sectionId}
          setAddState={setAddState}
          isEditing={false}
          setAddLessonClick={setAddLessonClick}
          setIsEditing={() => {}}
        />
      );
      break;
    case 'mcq':
      returnDiv = (
        <AddExam
          setIsEditing={() => {}}
          setAddState={() => setAddState('')}
          sectionId={sectionId}
        />
      );
      break;
    case 'assignment':
      returnDiv = (
        <AddAssignment
          setIsEditing={() => {}}
          setAddState={() => setAddState('')}
          sectionId={sectionId}
        />
      );
      break;
    case 'meeting':
      returnDiv = (
        <AddMeeting
          setIsEditing={() => {}}
          setAddState={setAddState}
          sectionId={sectionId}
          setAddLessonClick={setAddLessonClick}
          isEditing={false}
        />
      );
      break;
    case 'feedback':
      returnDiv = (
        <AddFeedback
          setIsEditing={() => {}}
          setAddState={setAddState}
          sectionId={sectionId}
        />
      );
      break;
    case 'document':
      returnDiv = (
        <AddDocument
          setIsEditing={() => {}}
          setAddState={setAddState}
          sectionId={sectionId}
          setAddLessonClick={setAddLessonClick}
        />
      );
      break;
    case 'physical':
      returnDiv = (
        <AddPhysical
          setIsEditing={() => {}}
          setAddState={setAddState}
          sectionId={sectionId}
          setAddLessonClick={setAddLessonClick}
        />
      );
      break;
    default:
      break;
  }
  return (
    <div>
      {!addState ? (
        <>
          <Badge
            style={{ cursor: 'pointer' }}
            variant="outline"
            onClick={() => setAddState('lecture')}
          >
            + {t('video')}
          </Badge>{' '}
          <Badge
            style={{ cursor: 'pointer' }}
            variant="outline"
            onClick={() => setAddState('mcq')}
          >
            + {t('exam')}
          </Badge>{' '}
          <Badge
            style={{ cursor: 'pointer' }}
            variant="outline"
            onClick={() => setAddState('assignment')}
          >
            + {t('assignment')}
          </Badge>{' '}
          <Badge
            style={{ cursor: 'pointer' }}
            variant="outline"
            onClick={() => setAddState('physical')}
          >
            + {t('physical_name')}
          </Badge>{' '}
          <Badge
            style={{ cursor: 'pointer' }}
            variant="outline"
            onClick={() => setAddState('meeting')}
          >
            + {t('live_class')}
          </Badge>{' '}
          <Badge
            style={{ cursor: 'pointer' }}
            variant="outline"
            onClick={() => setAddState('feedback')}
          >
            + {t('feedback')}
          </Badge>{' '}
          <Badge
            style={{ cursor: 'pointer' }}
            variant="outline"
            onClick={() => setAddState('document')}
          >
            + {t('document')}
          </Badge>{' '}
          <Badge
            color="red"
            style={{ cursor: 'pointer' }}
            variant="outline"
            onClick={() => setAddLessonClick(true)}
          >
            X {t('close')}
          </Badge>
        </>
      ) : (
        returnDiv
      )}
    </div>
  );
};

const AddLesson = ({ sectionId }: { sectionId: string }) => {
  const section = useSection();
  const [addLessonClick, setAddLessonClick] = useState<boolean>(true);
  const { t } = useTranslation();

  return (
    <div style={{ marginTop: '10px', marginBottom: '10px' }}>
      {addLessonClick ? (
        <Tooltip
          position="right"
          multiline
          w={220}
          label={t('click_to_see_options')}
        >
          <Button
            variant="outline"
            onClick={() => {
              // section?.setAddLessonClick(!section?.addLessonClick);

              setAddLessonClick(!addLessonClick);
              section?.setIsAddSection(false);
            }}
          >
            {t('add_lesson')}
          </Button>
        </Tooltip>
      ) : (
        <LessonAddList
          t={t}
          sectionId={sectionId}
          setAddLessonClick={setAddLessonClick}
        />
      )}
    </div>
  );
};

export default AddLesson;
