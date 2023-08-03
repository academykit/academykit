import TextEditor from '@components/Ui/TextEditor';
import ThumbnailEditor from '@components/Ui/ThumbnailEditor';
import {
  Box,
  Button,
  Group,
  Loader,
  MultiSelect,
  Select,
  Text,
} from '@mantine/core';
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
import { useAddTag, useTags } from '@utils/services/tagService';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useNavigate, useParams } from 'react-router-dom';
import * as Yup from 'yup';
import useFormErrorHooks from '@hooks/useFormErrorHooks';
import useCustomForm from '@hooks/useCustomForm';
import CustomTextFieldWithAutoFocus from '@components/Ui/CustomTextFieldWithAutoFocus';
import useAuth from '@hooks/useAuth';
import { UserRole } from '@utils/enums';

interface FormValues {
  thumbnail: string;
  title: string;
  level: any;
  groups: string;
  description: string;
  tags: string[];
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
    },
    validate: yupResolver(schema()),
  });
  useFormErrorHooks(form);

  const [searchParams] = useState('');
  const [searchParamsGroup] = useState('');
  const groupAdd = useAddGroup();
  const auth = useAuth();

  const label = useLevels();
  const { mutate, data: addTagData, isSuccess } = useAddTag();

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

  const [tagsList, setTagsList] = useState<{ value: string; label: string }[]>(
    []
  );

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
      setTagsList(tags.data.items.map((x) => ({ label: x.name, value: x.id })));
      courseSingleData?.tags &&
        form.setFieldValue('tags', [
          ...(form.values.tags ?? []),
          ...(courseSingleData?.tags?.map((x) => x.tagId) ?? []),
        ]);
    }
  }, [tags.isSuccess, courseIsSuccess]);

  useEffect(() => {
    if (isSuccess) {
      setTagsList([
        ...tagsList,
        { label: addTagData.data.name, value: addTagData.data.id },
      ]);
    }
  }, [isSuccess]);

  useEffect(() => {
    if (courseIsSuccess) {
      form.setValues({
        thumbnail: courseSingleData.thumbnailUrl,
        title: courseSingleData.name,
        description: courseSingleData.description,
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
        language: 1,
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
            />
            <Group mt={10} grow>
              <CustomTextFieldWithAutoFocus
                placeholder={t('title_course') as string}
                label={t('title')}
                withAsterisk
                {...form.getInputProps('title')}
                size="lg"
              />
            </Group>

            <Group grow mt={20}>
              {tags.isSuccess ? (
                <MultiSelect
                  searchable
                  creatable
                  sx={{ maxWidth: '500px' }}
                  data={tagsList}
                  {...form.getInputProps('tags')}
                  getCreateLabel={(query) => `+ Create ${query}`}
                  onCreate={(query) => {
                    mutate(query);
                    return null;
                  }}
                  size={'lg'}
                  label={t('tags')}
                  placeholder={t('tags_placeholder') as string}
                />
              ) : (
                <div>
                  <Loader size={'xs'} />
                </div>
              )}
              {label.isSuccess ? (
                <Select
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
            {!groups.isLoading ? (
              <Select
                mt={20}
                searchable
                withAsterisk
                description={<Text size={'xs'}>{t('group_create_info')}</Text>}
                sx={{ maxWidth: '500px' }}
                data={
                  groups?.data?.data?.items?.map((x) => ({
                    label: x.name,
                    value: x.id,
                  })) ?? []
                }
                {...form.getInputProps('groups')}
                size={'lg'}
                label={t('group')}
                placeholder={t('group_placeholder') as string}
                creatable={
                  // allow for admin and superadmin only
                  auth?.auth?.role == UserRole.SuperAdmin ||
                  auth?.auth?.role == UserRole.Admin
                }
                getCreateLabel={(query) => `+ Create ${query}`}
                onCreate={(value) => {
                  groupAdd
                    .mutateAsync(value)
                    .then((res) => form.setFieldValue('groups', res.data.id)); // setting value after fetch
                  return value;
                }}
                nothingFound="No options"
              />
            ) : (
              <Loader />
            )}
            <Box mt={20}>
              <Text>{t('description')}</Text>
              <TextEditor
                placeholder={t('course_description') as string}
                formContext={useFormContext}
              />
            </Box>
            <Box mt={20}>
              <Button
                disabled={!cForm?.isReady}
                size="lg"
                type="submit"
                loading={updateCourse.isLoading}
              >
                {t('submit')}
              </Button>
            </Box>
          </Box>
        </form>
      </FormProvider>
    </div>
  );
};

export default EditCourse;
