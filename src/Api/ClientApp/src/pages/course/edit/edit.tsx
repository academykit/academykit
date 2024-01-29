import { DynamicAutoFocusTextField } from '@components/Ui/CustomTextFieldWithAutoFocus';
import GroupCreatableSelect from '@components/Ui/GroupCreatableSelect';
import RichTextEditor from '@components/Ui/RichTextEditor/Index';
import TextViewer from '@components/Ui/RichTextViewer';
import ThumbnailEditor from '@components/Ui/ThumbnailEditor';
import useAuth from '@hooks/useAuth';
import useCustomForm from '@hooks/useCustomForm';
import useFormErrorHooks from '@hooks/useFormErrorHooks';
import { Box, Button, Group, Loader, Select, Text } from '@mantine/core';
import { createFormContext, yupResolver } from '@mantine/form';
import { showNotification } from '@mantine/notifications';
import queryStringGenerator from '@utils/queryStringGenerator';
import RoutePath from '@utils/routeConstants';
import errorType from '@utils/services/axiosError';
import {
  useCourseDescription,
  useUpdateCourse,
} from '@utils/services/courseService';
import { useAddGroup, useGroups } from '@utils/services/groupService';
import { useLevels } from '@utils/services/levelService';
import { ITag, useAddTag, useTags } from '@utils/services/tagService';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useNavigate, useParams } from 'react-router-dom';
import * as Yup from 'yup';
import TagMultiSelectCreatable from '../component/TagMultiSelectCreatable';

interface FormValues {
  thumbnail: string;
  title: string;
  level: any;
  groups: string;
  description: string;
  tags: string[];
  language: string;
}

const schema = () => {
  const { t } = useTranslation();
  return Yup.object().shape({
    title: Yup.string()
      .required(t('course_title_required') as string)
      .max(250, t('course_title_must_be_less_than_100') as string),
    level: Yup.string().required(t('level_required') as string),
    groups: Yup.string().required(t('group_required') as string),
  });
};

export const [FormProvider, useFormContext, useForm] =
  createFormContext<FormValues>();
const EditCourse = () => {
  const [viewMode, setViewMode] = useState(true);
  const cForm = useCustomForm();
  const { t } = useTranslation();
  const form = useForm({
    initialValues: {
      thumbnail: '',
      title: '',
      level: '',
      groups: '',
      description: '',
      tags: [],
      language: '1',
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  const [searchParams] = useState('');
  const [searchParamsGroup] = useState('');
  const groupAdd = useAddGroup();
  const auth = useAuth();

  const label = useLevels();
  const { data: addTagData, isSuccess, mutateAsync: mutAsync } = useAddTag();
  const [language] = useState([
    { value: '1', label: 'English' },
    { value: '2', label: 'Nepali' },
  ]);

  const tags = useTags(
    queryStringGenerator({
      search: searchParams,
      size: 10000,
    })
  );

  const groups = useGroups(
    queryStringGenerator({
      search: searchParamsGroup,
      size: 10000,
    })
  );

  const slug = useParams();
  const {
    data: courseSingleData,
    isSuccess: courseIsSuccess,
    refetch,
  } = useCourseDescription(slug.id as string);

  const [tagsLists, setTagsLists] = useState<ITag[]>([]);

  useEffect(() => {
    if (label.isSuccess) {
      form.setFieldValue('level', courseSingleData?.levelId);
    }
  }, [label.isSuccess, courseIsSuccess]);

  useEffect(() => {
    if (groups.isSuccess) {
      form.setFieldValue('groups', courseSingleData?.groupId as string);
    }
  }, [groups.isSuccess, courseIsSuccess]);

  useEffect(() => {
    if (tags.isSuccess) {
      // setTagsList(tags.data.items.map((x) => ({ label: x.name, value: x.id })));
      setTagsLists(tags.data.items.map((x) => x));
      courseSingleData?.tags &&
        form.setFieldValue('tags', [
          ...(form.values.tags ?? []),
          ...(courseSingleData?.tags?.map((x) => x.tagId) ?? []),
        ]);
    }
  }, [tags.isSuccess, courseIsSuccess]);

  useEffect(() => {
    if (isSuccess) {
      // setTagsList([
      //   ...tagsList,
      //   { label: addTagData.data.name, value: addTagData.data.id },
      // ]);
      setTagsLists([...tagsLists, addTagData.data]);
    }
  }, [isSuccess]);

  useEffect(() => {
    if (courseIsSuccess) {
      form.setValues({
        thumbnail: courseSingleData.thumbnailUrl,
        title: courseSingleData.name,
        description: courseSingleData.description,
        language: courseSingleData.language.toString(),
      });
    }
  }, [courseIsSuccess]);

  const updateCourse = useUpdateCourse(slug.id as string);
  const navigator = useNavigate();

  const submitHandler = async (values: typeof form.values) => {
    try {
      await updateCourse.mutateAsync({
        name: values.title,
        thumbnailUrl: values.thumbnail,
        description: values.description,
        groupId: values.groups,
        language: parseInt(values.language),
        duration: 0,
        levelId: values.level,
        tagIds: values.tags,
      });
      refetch();
      navigator(RoutePath.manageCourse.lessons(slug.id).route);
      showNotification({
        title: t('success'),
        message: t('training_update_success'),
      });
    } catch (err) {
      const error = errorType(err);
      showNotification({
        message: error,
        color: 'red',
      });
    }
  };

  return (
    <div>
      <FormProvider form={form}>
        <form onSubmit={form.onSubmit(submitHandler)}>
          <Box mt={20}>
            <ThumbnailEditor
              formContext={useFormContext}
              currentThumbnail={courseSingleData?.thumbnailUrl}
              label={t('thumbnail') as string}
              disabled={viewMode}
            />
            <Group mt={10} grow>
              <DynamicAutoFocusTextField
                isViewMode={viewMode}
                readOnly={viewMode}
                placeholder={t('title_course') as string}
                label={t('title')}
                withAsterisk
                {...form.getInputProps('title')}
                size="lg"
              />
            </Group>

            <Group grow mt={20}>
              {tags.isSuccess ? (
                <TagMultiSelectCreatable
                  data={tagsLists ?? []}
                  mutateAsync={mutAsync}
                  form={form}
                  existingTags={courseSingleData}
                  size="lg"
                  readOnly={viewMode}
                />
              ) : (
                <div>
                  <Loader size={'xs'} />
                </div>
              )}
              {label.isSuccess ? (
                <Select
                  styles={{
                    input: {
                      border: viewMode ? 'none' : '',
                      cursor: viewMode ? 'text !important' : '',
                    },
                  }}
                  readOnly={viewMode}
                  withAsterisk
                  size="lg"
                  label={t('level')}
                  placeholder={t('level_placeholder') as string}
                  {...form.getInputProps('level')}
                  data={label.data.map((x) => ({ value: x.id, label: x.name }))}
                ></Select>
              ) : (
                <div>
                  <Loader size={'xs'} />
                </div>
              )}
            </Group>
            <Group grow mt={20}>
              {!groups.isLoading ? (
                <GroupCreatableSelect
                  api={groupAdd}
                  form={form}
                  data={groups?.data?.data?.items}
                  size="lg"
                  auth={auth}
                  readOnly={viewMode}
                />
              ) : (
                <Loader style={{ flexGrow: '0' }} />
              )}

              <Select
                readOnly={viewMode}
                styles={{
                  input: {
                    border: viewMode ? 'none' : '',
                    cursor: viewMode ? 'text !important' : '',
                  },
                }}
                label={t('Language')}
                size={'lg'}
                data={language}
                {...form.getInputProps('language')}
              />
            </Group>
            <Box mt={20}>
              <Text>{t('description')}</Text>
              {!viewMode && (
                <RichTextEditor
                  placeholder={t('course_description') as string}
                  formContext={useFormContext}
                />
              )}
              {viewMode && (
                <TextViewer content={courseSingleData?.description as string} />
              )}
            </Box>
            <Box mt={20}>
              {viewMode && (
                <Button size="lg" onClick={() => setViewMode(false)}>
                  {t('edit')}
                </Button>
              )}

              {!viewMode && (
                <>
                  <Button
                    disabled={!cForm?.isReady}
                    size="lg"
                    type="submit"
                    loading={updateCourse.isLoading}
                  >
                    {t('submit')}
                  </Button>
                  <Button
                    ml={15}
                    size="lg"
                    onClick={() => setViewMode(true)}
                    variant="outline"
                  >
                    {t('cancel')}
                  </Button>
                </>
              )}
            </Box>
          </Box>
        </form>
      </FormProvider>
    </div>
  );
};

export default EditCourse;
