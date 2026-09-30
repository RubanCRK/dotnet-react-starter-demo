import { FormEvent, useCallback, useEffect, useState } from 'react';
import { executeApiRequest } from '../../lib/api';
import type {
  DisplayTheme,
  SingleItemEnvelope,
  UpdateUserPreferencesRequest,
  UserPreferences,
} from '../../lib/types';
import { LoadingSpinner } from '../../components/LoadingSpinner';
import { ErrorMessage } from '../../components/ErrorMessage';

/**
 * User-facing strings kept in one place so they remain extractable for i18n.
 */
const PAGE_TEXT = {
  heading: 'User Preferences',
  userIdLabel: 'User ID',
  themeLegend: 'Display Theme',
  themeLight: 'Light',
  themeDark: 'Dark',
  notificationsLabel: 'Enable notifications',
  saveButton: 'Save Preferences',
  savingButton: 'Saving…',
  savedMessage: 'Preferences saved.',
  loadErrorTitle: 'Preferences Unavailable',
  saveErrorTitle: 'Save Failed',
  unknownError: 'Unknown error occurred',
} as const;

const DEFAULT_USER_ID = 1;

type PageState = 'loading' | 'ready' | 'error';

/**
 * Applies the selected display theme to the document root so global
 * styles can react to it (e.g. via a `dark` class and Tailwind variants).
 */
function applyDisplayTheme(selectedTheme: DisplayTheme) {
  document.documentElement.classList.toggle('dark', selectedTheme === 'dark');
}

/**
 * User preferences page.
 * Loads and saves display theme and notification settings via the backend API.
 * Uses a manual user identifier until authentication is implemented.
 */
export function PreferencesPage() {
  const [pageState, setPageState] = useState<PageState>('loading');
  const [userId, setUserId] = useState<number>(DEFAULT_USER_ID);
  const [selectedTheme, setSelectedTheme] = useState<DisplayTheme>('light');
  const [notificationsEnabled, setNotificationsEnabled] = useState<boolean>(true);
  const [errorText, setErrorText] = useState<string>('');
  const [saveInProgress, setSaveInProgress] = useState<boolean>(false);
  const [saveConfirmation, setSaveConfirmation] = useState<string>('');

  const loadPreferences = useCallback(async (requestedUserId: number) => {
    setPageState('loading');
    setErrorText('');
    try {
      const apiResponse = await executeApiRequest<SingleItemEnvelope<UserPreferences>>(
        `/v1/users/${requestedUserId}/preferences`
      );
      setSelectedTheme(apiResponse.item.theme);
      setNotificationsEnabled(apiResponse.item.notificationsEnabledIndicator);
      applyDisplayTheme(apiResponse.item.theme);
      setPageState('ready');
    } catch (err) {
      setErrorText(err instanceof Error ? err.message : PAGE_TEXT.unknownError);
      setPageState('error');
    }
  }, []);

  useEffect(() => {
    loadPreferences(DEFAULT_USER_ID);
  }, [loadPreferences]);

  async function handleSave(submitEvent: FormEvent<HTMLFormElement>) {
    submitEvent.preventDefault();
    setSaveInProgress(true);
    setSaveConfirmation('');
    setErrorText('');

    const requestPayload: UpdateUserPreferencesRequest = {
      theme: selectedTheme,
      notificationsEnabledIndicator: notificationsEnabled,
    };

    try {
      const apiResponse = await executeApiRequest<SingleItemEnvelope<UserPreferences>>(
        `/v1/users/${userId}/preferences`,
        {
          method: 'PUT',
          body: JSON.stringify(requestPayload),
        }
      );
      applyDisplayTheme(apiResponse.item.theme);
      setSaveConfirmation(PAGE_TEXT.savedMessage);
    } catch (err) {
      setErrorText(err instanceof Error ? err.message : PAGE_TEXT.unknownError);
    } finally {
      setSaveInProgress(false);
    }
  }

  function handleUserIdChange(nextUserId: number) {
    setUserId(nextUserId);
    setSaveConfirmation('');
    if (nextUserId > 0) {
      loadPreferences(nextUserId);
    }
  }

  if (pageState === 'loading') {
    return <LoadingSpinner />;
  }

  if (pageState === 'error') {
    return (
      <ErrorMessage
        errorTitle={PAGE_TEXT.loadErrorTitle}
        errorDescription={errorText}
      />
    );
  }

  return (
    <div className="space-y-6">
      <h2 className="text-2xl font-bold text-[var(--color-brand-text)]">
        {PAGE_TEXT.heading}
      </h2>

      <form
        onSubmit={handleSave}
        className="bg-white rounded-lg shadow-md p-6 space-y-6"
      >
        <div className="flex items-center gap-4">
          <label htmlFor="user-id-input" className="text-gray-600 font-medium w-32">
            {PAGE_TEXT.userIdLabel}
          </label>
          <input
            id="user-id-input"
            type="number"
            min={1}
            value={userId}
            onChange={(changeEvent) => handleUserIdChange(Number(changeEvent.target.value))}
            className="border border-gray-300 rounded-md px-3 py-2 w-32 focus:outline-none focus:ring-2 focus:ring-[var(--color-brand-primary)]"
          />
        </div>

        <fieldset>
          <legend className="text-gray-600 font-medium mb-2">{PAGE_TEXT.themeLegend}</legend>
          <div className="flex gap-6">
            <label className="flex items-center gap-2 cursor-pointer">
              <input
                type="radio"
                name="display-theme"
                value="light"
                checked={selectedTheme === 'light'}
                onChange={() => setSelectedTheme('light')}
                className="accent-[var(--color-brand-primary)]"
              />
              <span>{PAGE_TEXT.themeLight}</span>
            </label>
            <label className="flex items-center gap-2 cursor-pointer">
              <input
                type="radio"
                name="display-theme"
                value="dark"
                checked={selectedTheme === 'dark'}
                onChange={() => setSelectedTheme('dark')}
                className="accent-[var(--color-brand-primary)]"
              />
              <span>{PAGE_TEXT.themeDark}</span>
            </label>
          </div>
        </fieldset>

        <label className="flex items-center gap-3 cursor-pointer">
          <input
            type="checkbox"
            checked={notificationsEnabled}
            onChange={(changeEvent) => setNotificationsEnabled(changeEvent.target.checked)}
            className="w-4 h-4 accent-[var(--color-brand-primary)]"
          />
          <span className="text-[var(--color-brand-text)]">{PAGE_TEXT.notificationsLabel}</span>
        </label>

        <div className="flex items-center gap-4">
          <button
            type="submit"
            disabled={saveInProgress}
            className="bg-[var(--color-brand-primary)] text-white px-6 py-2 rounded-lg font-medium hover:bg-blue-700 transition-colors disabled:opacity-50"
          >
            {saveInProgress ? PAGE_TEXT.savingButton : PAGE_TEXT.saveButton}
          </button>
          {saveConfirmation && (
            <span className="text-green-700 font-medium">{saveConfirmation}</span>
          )}
          {errorText && (
            <span className="text-red-700 font-medium">{errorText}</span>
          )}
        </div>
      </form>
    </div>
  );
}
